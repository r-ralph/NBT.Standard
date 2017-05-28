using System;
using System.IO;
using NBT.Serialization;
using Xunit;

namespace NBT.Test.Serialization
{
    public class TagWriterTests
    {
        #region  Tests

        [Fact]
        public void CreateWriter_returns_binary_writer()
        {
            // act
            var target = TagWriter.CreateWriter(NbtFormat.Binary, new MemoryStream());

            // assert
            Assert.IsType<BinaryTagWriter>(target);
        }

        [Fact]
        public void CreateWriter_returns_xml_writer()
        {
            // act
            var target = TagWriter.CreateWriter(NbtFormat.Xml, new MemoryStream());

            // assert
            Assert.IsType<XmlTagWriter>(target);
        }

        [Fact]
        public void CreateWriter_throws_exception_for_null_stream()
        {
            // act
            var e = Assert.Throws<ArgumentNullException>(() => TagWriter.CreateWriter(NbtFormat.Xml, null));
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: stream", e.Message);
        }

        [Fact]
        public void CreateWriter_throws_exception_for_unknown_type()
        {
            // act
            var e = Assert.Throws<ArgumentOutOfRangeException>(
                () => TagWriter.CreateWriter(NbtFormat.Unknown, new MemoryStream()));
            Assert.Equal($"Invalid format.{Environment.NewLine}" +
                         $"Parameter name: format{Environment.NewLine}" +
                         "Actual value was Unknown.",
                e.Message);
        }

        #endregion
    }
}