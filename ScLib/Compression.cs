using System;
using System.IO;
using LZMA.Compress;
using LZMA.Compress.LZMA;
using static ScLib.States;

namespace ScLib
{
    public class Compression : IDisposable
    {
        private static readonly int dictionary = 1 << 21;
        private static readonly int posStateBits = 2;
        private static readonly int litContextBits = 3;
        private static readonly int litPosBits = 0;
        private static readonly int algorithm = 2;
        private static readonly int numFastBytes = 128;
        private static readonly bool eos = false;
        private static readonly string mf = "bt4";

        private readonly object[] properties =
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

        public readonly CoderPropID[] propIDs =
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

        public byte[] Decompress(Stream inStream, CompressionType Type)
        {
            var stream = new MemoryStream();
            var decoder = new Decoder();

            if (Type == CompressionType.Lzmha)
                inStream.Seek(26, SeekOrigin.Current);

            var properties = new byte[5];
            if (inStream.Read(properties, 0, 5) < 5)
                throw new Exception("Invalid properties.");

            var buffer = new byte[4];
            inStream.Read(buffer, 0, 4);

            decoder.SetDecoderProperties(properties);
            decoder.Code(inStream, stream, inStream.Length, BitConverter.ToInt32(buffer, 0), null);

            inStream.Flush();

            return stream.ToArray();
        }

        /*public byte[] Compress(Stream inStream, CompressionType Type)
        {
            var stream = new MemoryStream();
            var encoder = new Encoder();

            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(stream);

            if (BitConverter.IsLittleEndian)
            {
                var LengthHeader = BitConverter.GetBytes(inStream.Length);
                stream.Write(LengthHeader, 0, LengthHeader.Length);
            }

            encoder.Code(inStream, stream, inStream.Length, -1, null);

            return stream.ToArray();
        }*/

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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