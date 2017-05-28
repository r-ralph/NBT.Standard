using System;
using System.IO;
using NBT.Serialization;
using Xunit;

namespace NBT.Test.Serialization
{
    public partial class BinaryTagWriterTests : TestBase
    {
        #region  Tests

        [Fact]
        public void WriteValue_throws_exception_for_long_strings()
        {
            // arrange
            var stream = new MemoryStream();
            var target = CreateWriter(stream);

            target.WriteStartDocument();
            target.WriteStartTag(TagType.Compound);

            // act
            var e = Assert.Throws<ArgumentException>(() => target.WriteTag(new string(' ', short.MaxValue + 1)));
            Assert.Equal("String data would be truncated.", e.Message);
        }

        #endregion

        #region Test Helpers

        private TagReader CreateReader(Stream stream)
        {
            return new BinaryTagReader(stream, false);
        }

        private TagWriter CreateWriter(Stream stream)
        {
            return new BinaryTagWriter(stream);
        }

        #endregion
    }
}