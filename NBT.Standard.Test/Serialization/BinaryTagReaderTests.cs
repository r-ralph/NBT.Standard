using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NBT.Serialization;
using Xunit;

namespace NBT.Test.Serialization
{
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
    public partial class BinaryTagReaderTests : TestBase
    {
        #region  Tests

        [Fact]
        public void IsNbtDocument_handles_none_seekable_streams()
        {
            // arrange
            bool actual;

            var target = CreateReader(new NoSeekStream(File.ReadAllBytes(SimpleDataFileName)));

            // act
            actual = target.IsNbtDocument();

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void ReadDocument_should_handle_deflate_compressed_files()
        {
            // arrange

            var expected = CreateComplexData();
            Stream stream = File.OpenRead(DeflateComplexDataFileName);
            TagReader target = new BinaryTagReader(stream);

            // act
            var actual = target.ReadDocument();

            // assert
            NbtAssert.Equal(expected, actual);
        }

        [Fact]
        public void ReadDocument_should_handle_gzip_compressed_files()
        {
            // arrange
            var expected = CreateComplexData();
            Stream stream = File.OpenRead(ComplexDataFileName);
            TagReader target = new BinaryTagReader(stream);

            // act
            var actual = target.ReadDocument();

            // assert
            NbtAssert.Equal(expected, actual);
        }

        [Fact]
        public void ReadDocument_should_handle_uncompressed_files()
        {
            // arrange
            var expected = CreateComplexData();
            Stream stream = File.OpenRead(UncompressedComplexDataFileName);
            TagReader target = new BinaryTagReader(stream);

            // act
            var actual = target.ReadDocument();

            // assert
            NbtAssert.Equal(expected, actual);
        }

        [Fact]
        public void ReadList_throws_exception_if_list_type_is_invalid()
        {
            using (var stream = new MemoryStream())
            {
                // arrange
                var reader = CreateReader(stream);
                TagWriter writer = new BinaryTagWriter(stream);

                writer.WriteStartDocument();
                writer.WriteStartTag("list", TagType.List, (TagType) 182, 0);
                writer.WriteEndTag();
                writer.WriteEndDocument();

                stream.Position = 0;

                reader.ReadTagType();
                reader.ReadTagName();

                // act
                Exception e = Assert.Throws<InvalidDataException>(() => reader.ReadList());
                Assert.Equal("Unexpected list type '182' found.", e.Message);
            }
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

        private void WriteValue(Stream stream, int value)
        {
            var buffer = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                BitHelper.SwapBytes(buffer, 0, BitHelper.IntSize);
            }

            stream.Write(buffer, 0, BitHelper.IntSize);
        }

        private void WriteValue(Stream stream, short value)
        {
            var buffer = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
            {
                BitHelper.SwapBytes(buffer, 0, BitHelper.ShortSize);
            }

            stream.Write(buffer, 0, BitHelper.ShortSize);
        }

        #endregion
    }
}