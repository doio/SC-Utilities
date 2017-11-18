using System.IO;

namespace LZMA.Compress.LZ
{
    public class InWindow
    {
        public uint _blockSize;
        public byte[] _bufferBase;

        public uint _bufferOffset;
        private uint _keepSizeAfter;
        private uint _keepSizeBefore;

        private uint _pointerToLastSafePosition;
        public uint _pos;
        private uint _posLimit;
        private Stream _stream;
        private bool _streamEndWasReached;
        public uint _streamPos;

        public void MoveBlock()
        {
            var offset = _bufferOffset + _pos - _keepSizeBefore;

            if (offset > 0)
                offset--;

            for (uint i = 0; i < _bufferOffset + _streamPos - offset; i++)
                _bufferBase[i] = _bufferBase[offset + i];
            _bufferOffset -= offset;
        }

        public virtual void ReadBlock()
        {
            if (_streamEndWasReached)
                return;
            while (true)
            {
                var size = (int) (0 - _bufferOffset + _blockSize - _streamPos);
                if (size == 0)
                    return;
                var numReadBytes = _stream.Read(_bufferBase, (int) (_bufferOffset + _streamPos), size);
                if (numReadBytes == 0)
                {
                    _posLimit = _streamPos;
                    var pointerToPostion = _bufferOffset + _posLimit;
                    if (pointerToPostion > _pointerToLastSafePosition)
                        _posLimit = _pointerToLastSafePosition - _bufferOffset;

                    _streamEndWasReached = true;
                    return;
                }
                _streamPos += (uint) numReadBytes;
                if (_streamPos >= _pos + _keepSizeAfter)
                    _posLimit = _streamPos - _keepSizeAfter;
            }
        }

        private void Free()
        {
            _bufferBase = null;
        }

        public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
        {
            _keepSizeBefore = keepSizeBefore;
            _keepSizeAfter = keepSizeAfter;
            var blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
            if (_bufferBase == null || _blockSize != blockSize)
            {
                Free();
                _blockSize = blockSize;
                _bufferBase = new byte[_blockSize];
            }
            _pointerToLastSafePosition = _blockSize - keepSizeAfter;
        }

        public void SetStream(Stream stream)
        {
            _stream = stream;
        }

        public void ReleaseStream()
        {
            _stream = null;
        }

        public void Init()
        {
            _bufferOffset = 0;
            _pos = 0;
            _streamPos = 0;
            _streamEndWasReached = false;
            ReadBlock();
        }

        public void MovePos()
        {
            _pos++;
            if (_pos > _posLimit)
            {
                var pointerToPostion = _bufferOffset + _pos;
                if (pointerToPostion > _pointerToLastSafePosition)
                    MoveBlock();
                ReadBlock();
            }
        }

        public byte GetIndexByte(int index)
        {
            return _bufferBase[_bufferOffset + _pos + index];
        }

        public uint GetMatchLen(int index, uint distance, uint limit)
        {
            if (_streamEndWasReached)
                if (_pos + index + limit > _streamPos)
                    limit = _streamPos - (uint) (_pos + index);
            distance++;
            var pby = _bufferOffset + _pos + (uint) index;

            uint i;
            for (i = 0; i < limit && _bufferBase[pby + i] == _bufferBase[pby + i - distance]; i++) ;
            return i;
        }

        public uint GetNumAvailableBytes()
        {
            return _streamPos - _pos;
        }

        public void ReduceOffsets(int subValue)
        {
            _bufferOffset += (uint) subValue;
            _posLimit -= (uint) subValue;
            _pos -= (uint) subValue;
            _streamPos -= (uint) subValue;
        }
    }
}