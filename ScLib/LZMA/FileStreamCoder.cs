using System;
using System.IO;
using LZMA.Compress.LZMA;

namespace LZMA.Compress
{
    public class LZMACoder : IDisposable
    {
        private static readonly int dictionary = 1 << 21; 
        private static readonly int posStateBits = 2;

        private static readonly int litContextBits = 3;
        private static readonly int litPosBits = 0; 
        private static readonly int algorithm = 2;
        private static readonly int numFastBytes = 128;
        private static readonly bool eos = false;
        private static readonly string mf = "bt4";

        private static readonly CoderPropID[] propIDs =
        {
            CoderPropID.DictionarySize,
            CoderPropID.PosStateBits,
            CoderPropID.LitContextBits,
            CoderPropID.LitPosBits,
            CoderPropID.Algorithm,
            CoderPropID.NumFastBytes,
            CoderPropID.MatchFinder,
            CoderPropID.EndMarker
        };

        private static readonly object[] properties =
        {
            dictionary,
            posStateBits,
            litContextBits,
            litPosBits,
            algorithm,
            numFastBytes,
            mf,
            eos
        };

        private bool isDisposed;

        public LZMACoder()
        {
            if (BitConverter.IsLittleEndian == false)
            {
                Dispose();
                throw new Exception("Not implemented");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Decompress(Stream inStream, Stream outStream)
        {
            Decompress(inStream, outStream, false);
        }

        public void Decompress(Stream inStream, Stream outStream, bool closeInStream)
        {
            inStream.Position = 0;

            var properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw new Exception("input .lzma is too short");

            var decoder = new Decoder();
            decoder.SetDecoderProperties(properties);

            long outSize = 0;

            if (BitConverter.IsLittleEndian)
                for (var i = 0; i < 8; i++)
                {
                    var v = inStream.ReadByte();
                    if (v < 0)
                        throw new Exception("Can't Read 1");

                    outSize |= (long) (byte) v << (8 * i);
                }

            var compressedSize = inStream.Length - inStream.Position;
            decoder.Code(inStream, outStream, compressedSize, outSize, null);

            if (closeInStream)
                inStream.Close();
        }

        public void Compress(Stream inStream, Stream outStream)
        {
            Compress(inStream, outStream, false);
        }

        public void Compress(Stream inStream, Stream outStream, bool closeInStream)
        {
            inStream.Position = 0;
            var fileSize = inStream.Length;

            var encoder = new Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);

            if (BitConverter.IsLittleEndian)
            {
                var LengthHeader = BitConverter.GetBytes(fileSize);
                outStream.Write(LengthHeader, 0, LengthHeader.Length);
            }

            encoder.Code(inStream, outStream, -1, -1, null);

            if (closeInStream)
                inStream.Close();
        }

        ~LZMACoder()
        {
            Dispose();
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed == false)
                if (disposing)
                    GC.SuppressFinalize(this);

            isDisposed = true;
        }
    }
}