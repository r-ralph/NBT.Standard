using System.IO;
using System.Xml;
using NBT.Serialization;
using Xunit;

namespace NBT.Test.Serialization
{
    public partial class XmlTagWriterTests : TestBase
    {
        #region  Tests

        [Fact]
        public void Close_should_close_writer()
        {
            // arrange
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream);

            TagWriter target = new XmlTagWriter(writer);

            var expected = WriteState.Closed;

            // act
            target.Dispose();

            // assert
            Assert.Equal(expected, writer.WriteState);
        }

        [Fact]
        public void Constructor_allows_external_writer()
        {
            // arrange
            Tag actual;

            Tag expected = CreateComplexData();

            TextWriter textWriter = new StringWriter();
            var writer = XmlWriter.Create(textWriter, new XmlWriterSettings
            {
                Indent = true
            });

            TagWriter target = new XmlTagWriter(writer);

            // act
            target.WriteStartDocument();
            target.WriteTag(expected);
            target.WriteEndDocument();

            using (TextReader textReader = new StringReader(textWriter.ToString()))
            {
                using (var reader = XmlReader.Create(textReader))
                {
                    actual = new XmlTagReader(reader).ReadTag();
                }
            }

            // assert
            NbtAssert.Equal(expected, actual);
        }

        [Fact]
        public void WriteValue_should_use_cdata_if_required()
        {
            // arrange
            TextWriter writer = new StringWriter();
            var xmlWriter = XmlWriter.Create(writer);

            var expected =
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" +
                "<tag type=\"Compound\">" +
                "<alpha type=\"String\">" +
                "<![CDATA[<BAD>]]>" +
                "</alpha>" +
                "</tag>";

            var target = new XmlTagWriter(xmlWriter);
            target.WriteStartDocument();
            target.WriteStartTag(TagType.Compound);

            // act
            target.WriteTag("alpha", "<BAD>");

            // assert
            target.WriteEndTag();
            target.WriteEndDocument();
            var actual = writer.ToString();
            Assert.Equal(expected, actual);
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