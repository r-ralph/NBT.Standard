using System.IO;

namespace NBT.Test.Serialization
{
    internal sealed class NoSeekStream : MemoryStream
    {
        #region Constructors

        public NoSeekStream(byte[] buffer)
            : base(buffer)
        {
        }

        #endregion

        #region Properties

        public override bool CanSeek => false;

        #endregion
    }
}