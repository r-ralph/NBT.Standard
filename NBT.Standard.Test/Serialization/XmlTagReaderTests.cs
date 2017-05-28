using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using NBT.Serialization;
using Xunit;

namespace NBT.Test.Serialization
{
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
    public partial class XmlTagReaderTests : TestBase
    {
        #region  Tests

        [Fact]
        public void Close_should_close_reader()
        {
            // arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(
                @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<Level type=""Int"" />
"));
            var reader = XmlReader.Create(stream);

            TagReader target = new XmlTagReader(reader);

            var expected = ReadState.Closed;

            // act
            target.Dispose();

            // assert
            Assert.Equal(expected, reader.ReadState);
        }

        [Fact]
        public void Constructor_allows_external_reader()
        {
            // arrange
            Tag expected = CreateComplexData();

            var reader = XmlReader.Create(ComplexXmlDataFileName);

            TagReader target = new XmlTagReader(reader);

            // act
            var actual = target.ReadTag();

            // assert
            NbtAssert.Equal(expected, actual);
        }

        [Fact]
        public void IsNbtDocument_returns_false_for_non_compound_type()
        {
            // arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(
                @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<Level type=""Int"" />
"));

            var target = new XmlTagReader(stream);

            // act
            var actual = target.IsNbtDocument();

            // assert
            Assert.False(actual);
        }

        [Fact]
        public void LoadTest()
        {
            // arrange
            var expected = CreateComplexData();
            Stream stream = File.OpenRead(ComplexXmlDataFileName);
            TagReader target = new XmlTagReader(stream);

            // act
            var actual = target.ReadDocument();

            // assert
            NbtAssert.Equal(expected, actual);
        }

        [Fact]
        public void ReadDocument_can_handle_xml_documents_with_self_closing_tags()
        {
            // arrange
            Tag expected = CreateSimpleNesting();
            Stream stream = File.OpenRead(Path.Combine(DataPath, "project.xml"));
            var target = new XmlTagReader(stream);

            // act
            Tag actual = target.ReadDocument();

            // assert
            NbtAssert.Equal(expected, actual);
        }

        [Fact]
        public void ReadDocument_can_handle_xml_documents_without_whitespace()
        {
            // arrange
            var expected = CreateComplexData();
            Stream stream = File.OpenRead(ComplexXmlWithoutWhitespaceDataFileName);
            TagReader target = new XmlTagReader(stream);

            // act
            var actual = target.ReadDocument();

            // assert
            NbtAssert.Equal(expected, actual);
        }

        [Fact]
        public void ReadDocument_stuck_in_infinite_loop_for_empty_root()
        {
            using (Stream stream = new MemoryStream())
            {
                // arrange
                var writer = CreateWriter(stream);

                writer.WriteStartDocument();
                writer.WriteStartTag(TagType.Compound);
                writer.WriteEndTag();
                writer.WriteEndDocument();

                stream.Position = 0;

                var target = CreateReader(stream);

                // act
                // if the root element was empty, the statement below
                // would get stuck in an infinite loop, causing the test
                // time out after one minute
                Tag actual = target.ReadDocument();

                // assert
                Assert.NotNull(actual);
            }
        }

        [Fact]
        public void ReadList_throws_exception_if_list_type_not_set()
        {
            // arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(
                @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<Level type=""Compound"">
   <tag name=""listTest (long)"" type=""List"">
    <tag>11</tag>
    <tag>12</tag>
    <tag>13</tag>
    <tag>14</tag>
    <tag>15</tag>
  </tag>
</Level>"));
            var reader = XmlReader.Create(stream);
            TagReader target = new XmlTagReader(reader);

            // act
            var e = Assert.Throws<InvalidDataException>(() => target.ReadDocument());
            Assert.Equal("Missing limitType attribute, unable to determine list contents type.", e.Message);
        }

        [Fact]
        public void ReadTagType_throws_exception_if_list_type_not_set()
        {
            // arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(
                @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<Level type=""Compound"">
   <tag name=""listTest (long)"">
    <tag>11</tag>
    <tag>12</tag>
    <tag>13</tag>
    <tag>14</tag>
    <tag>15</tag>
  </tag>
</Level>"));
            var reader = XmlReader.Create(stream);
            TagReader target = new XmlTagReader(reader);

            // act
            var e = Assert.Throws<InvalidDataException>(() => target.ReadDocument());
            Assert.Equal("Missing type attribute, unable to determine tag type.", e.Message);
        }

        [Fact]
        public void ReadTagType_throws_exception_tag_type_is_unknown()
        {
            // arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(
                @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<Level type=""Compound"">
   <tag name=""listTest (long)"" type=""NOTATAG"">
    <tag>11</tag>
    <tag>12</tag>
    <tag>13</tag>
    <tag>14</tag>
    <tag>15</tag>
  </tag>
</Level>"));
            var reader = XmlReader.Create(stream);
            TagReader target = new XmlTagReader(reader);

            // act
            var e = Assert.Throws<InvalidDataException>(() => target.ReadDocument());
            Assert.Equal("Unrecognized or unsupported tag type 'NOTATAG'.", e.Message);
        }

        #endregion

        #region Test Helpers

        private TagReader CreateReader(Stream stream)
        {
            return new XmlTagReader(stream);
        }

        private TagWriter CreateWriter(Stream stream)
        {
            return new XmlTagWriter(stream);
        }

        #endregion
    }
}