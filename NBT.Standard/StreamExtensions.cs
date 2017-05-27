using System.IO;

namespace NBT
{
    internal static class StreamExtensions
    {
        #region Static Methods

        public static bool IsDeflateCompressed(this Stream stream)
        {
            // http://www.gzip.org/zlib/rfc-deflate.html#spec
            var position = stream.Position;
            var buffer = stream.ReadByte();
            var result = buffer != -1;

            if (result)
            {
                var header = (byte) buffer;
                var bit1Set = (header & (1 << 0)) != 0;
                var bit2Set = (header & (1 << 1)) != 0;
                var bit3Set = (header & (1 << 2)) != 0;
                result = bit1Set && (bit2Set || bit3Set) && !(bit2Set && bit3Set);
            }

            stream.Position = position;

            return result;
        }

        public static bool IsGzipCompressed(this Stream stream)
        {
            // http://www.gzip.org/zlib/rfc-gzip.html#file-format
            var position = stream.Position;
            var buffer = new byte[4];
            var bytesRead = stream.Read(buffer, 0, 4);
            var result = bytesRead == 4;

            if (result)
            {
                result = buffer[0] == 31 && buffer[1] == 139 && buffer[2] == 8;
            }

            stream.Position = position;

            return result;
        }

        #endregion
    }
}