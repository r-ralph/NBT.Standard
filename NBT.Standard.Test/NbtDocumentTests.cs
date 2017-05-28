using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Xunit;

namespace NBT.Test
{
    public class NbtDocumentTests : TestBase
    {
        #region  Tests

        [Fact]
        public void Constructor_should_have_default_format()
        {
            // arrange
            var expected = NbtFormat.Binary;

            // act
            var target = new NbtDocument();

            // assert
            Assert.Equal(expected, target.Format);
        }

        [Fact]
        public void Constructor_should_have_have_empty_root()
        {
            // arrange

            // act
            var target = new NbtDocument();

            // assert
            Assert.NotNull(target.DocumentRoot);
        }

        [Fact]
        public void Constructor_throws_exception_if_compound_is_null()
        {
            // act
            var e = Assert.Throws<ArgumentNullException>(() => new NbtDocument(null));
            Assert.Equal("Value cannot be null.\r\nParameter name: document", e.Message);
        }

        [Fact]
        public void EmptyListXmlTest()
        {
            // arrange
            NbtDocument reloaded;

            var fileName = GetWorkFile();
            var target = new NbtDocument
            {
                Format = NbtFormat.Xml
            };
            target.DocumentRoot.Name = "Test";
            target.DocumentRoot.Value.Add("EmptyList", TagType.List, TagType.Compound);

            // act
            try
            {
                target.Save(fileName);
                reloaded = NbtDocument.LoadDocument(fileName);
            }
            finally
            {
                DeleteFile(fileName);
            }

            // assert
            // this test is essentially ensuring that an infinite loop when reloading an XML document is no longer present
            NbtAssert.Equal(target.DocumentRoot, reloaded.DocumentRoot);
        }

        [Fact]
        public void FormatTest()
        {
            // arrange
            NbtDocument expected;
            NbtDocument target;
            bool file1IsBinary;
            bool file2IsXml;

            var fileName1 = GetWorkFile();
            var fileName2 = GetWorkFile();
            var source = new NbtDocument(CreateComplexData());

            // act
            try
            {
                source.Format = NbtFormat.Binary;
                source.Save(fileName1);
                source.Format = NbtFormat.Xml;
                source.Save(fileName2);

                expected = NbtDocument.LoadDocument(fileName1);
                target = NbtDocument.LoadDocument(fileName2);

                file1IsBinary = expected.Format == NbtFormat.Binary;
                file2IsXml = target.Format == NbtFormat.Xml;
            }
            finally
            {
                DeleteFile(fileName1);
                DeleteFile(fileName2);
            }

            // assert
            Assert.True(file1IsBinary);
            Assert.True(file2IsXml);
            NbtAssert.Equal(expected, target);
        }

        [Fact]
        public void Get_document_name_should_return_name_from_deflate_binary_file()
        {
            // arrange
            const string expected = "Level";

            // act
            var actual = NbtDocument.GetDocumentName(DeflateComplexDataFileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Get_document_name_should_return_name_from_gzip_binary_file()
        {
            // arrange
            const string expected = "Level";

            // act
            var actual = NbtDocument.GetDocumentName(ComplexDataFileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Get_document_name_should_return_name_from_uncompressed_binary_file()
        {
            // arrange
            const string expected = "Level";

            // act
            var actual = NbtDocument.GetDocumentName(UncompressedComplexDataFileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Get_document_name_should_return_name_from_xml_file()
        {
            // arrange
            const string expected = "Level";

            // act
            var actual = NbtDocument.GetDocumentName(ComplexXmlDataFileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDocumentFormat_should_throw_exception_if_file_not_found()
        {
            // arrange
            var fileName = Guid.NewGuid().ToString();

            // act
            Assert.Throws<FileNotFoundException>(() => NbtDocument.GetDocumentFormat(fileName));
        }

        [Fact]
        public void GetDocumentFormat_should_throw_exception_with_null_filename()
        {
            // arrange

            // act
            Assert.Throws<ArgumentNullException>(() => NbtDocument.GetDocumentFormat((string) null));

            // assert
        }

        [Fact]
        public void GetDocumentFormat_should_throw_exception_with_null_stream()
        {
            // arrange

            // act
            Assert.Throws<ArgumentNullException>(() => NbtDocument.GetDocumentFormat((Stream) null));

            // assert
        }

        [Fact]
        public void GetDocumentFormat_throws_exception_for_non_seekable_streams()
        {
            // arrange
            Stream stream = new DeflateStream(Stream.Null, CompressionMode.Decompress);

            // act
            var e = Assert.Throws<ArgumentNullException>(() => NbtDocument.GetDocumentFormat(stream));
            Assert.Equal("Stream is not seekable.\r\nParameter name: stream", e.Message);
        }

        [Fact]
        public void GetDocumentName_should_throw_exception_if_file_not_found()
        {
            // arrange
            var fileName = Guid.NewGuid().ToString("N");

            // act
            Assert.Throws<FileNotFoundException>(() => NbtDocument.GetDocumentName(fileName));

            // assert
        }

        [Fact]
        public void GetDocumentName_should_throw_exception_if_filename_is_empty()
        {
            // arrange

            // act
            Assert.Throws<ArgumentNullException>(() => NbtDocument.GetDocumentName(string.Empty));

            // assert
        }

        [Fact]
        public void GetDocumentName_should_throw_exception_if_filename_is_null()
        {
            // arrange

            // act
            Assert.Throws<ArgumentNullException>(() => NbtDocument.GetDocumentName(null));

            // assert
        }

        [Fact]
        public void GetDocumentNameBadFileTest()
        {
            // arrange
            var fileName = BadFileName;

            // act
            var actual = NbtDocument.GetDocumentName(fileName);

            // assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetDocumentNameTest()
        {
            // arrange
            const string expected = "hello world";
            var fileName = SimpleDataFileName;

            // act
            var actual = NbtDocument.GetDocumentName(fileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFormatBinaryTest()
        {
            // arrange
            var fileName = UncompressedComplexDataFileName;
            const NbtFormat expected = NbtFormat.Binary;

            // act
            var actual = NbtDocument.GetDocumentFormat(fileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFormatDeflateBinaryTest()
        {
            // arrange
            var fileName = DeflateComplexDataFileName;
            const NbtFormat expected = NbtFormat.Binary;

            // act
            var actual = NbtDocument.GetDocumentFormat(fileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFormatGzipBinaryTest()
        {
            // arrange
            var fileName = ComplexDataFileName;
            const NbtFormat expected = NbtFormat.Binary;

            // act
            var actual = NbtDocument.GetDocumentFormat(fileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFormatInvalidTest()
        {
            // arrange
            var fileName = BadFileName;
            const NbtFormat expected = NbtFormat.Unknown;

            // act
            var actual = NbtDocument.GetDocumentFormat(fileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFormatXmlTest()
        {
            // arrange
            var fileName = ComplexXmlDataFileName;
            const NbtFormat expected = NbtFormat.Xml;

            // act
            var actual = NbtDocument.GetDocumentFormat(fileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InvalidFormatTest()
        {
            // arrange
            var target = new NbtDocument();

            // act
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Format = (NbtFormat) (-1));

            // assert
        }

        [Fact]
        public void IsNbtDocument_should_return_false_for_unknown_file()
        {
            // arrange
            var fileName = BadFileName;

            // act
            var actual = NbtDocument.IsNbtDocument(fileName);

            // assert
            Assert.False(actual);
        }

        [Fact]
        public void IsNbtDocument_should_return_true_for_deflate_binary_file()
        {
            // arrange
            var fileName = DeflateComplexDataFileName;

            // act
            var actual = NbtDocument.IsNbtDocument(fileName);

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void IsNbtDocument_should_return_true_for_gzip_binary_file()
        {
            // arrange
            var fileName = ComplexDataFileName;

            // act
            var actual = NbtDocument.IsNbtDocument(fileName);

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void IsNbtDocument_should_return_true_for_gzip_binary_stream()
        {
            // arrange
            bool actual;
            var fileName = ComplexDataFileName;

            // act
            using (Stream stream = File.OpenRead(fileName))
            {
                actual = NbtDocument.IsNbtDocument(stream);
            }

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void IsNbtDocument_should_return_true_for_uncompressed_binary_file()
        {
            // arrange
            var fileName = UncompressedComplexDataFileName;

            // act
            var actual = NbtDocument.IsNbtDocument(fileName);

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void IsNbtDocument_should_return_true_for_xml_file()
        {
            // arrange
            var fileName = ComplexXmlDataFileName;

            // act
            var actual = NbtDocument.IsNbtDocument(fileName);

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void IsNbtDocument_should_throw_exception_for_null_filename()
        {
            // arrange

            // act

            Assert.Throws<ArgumentNullException>(() => NbtDocument.IsNbtDocument((string) null));

            // assert
        }

        [Fact]
        public void IsNbtDocument_should_throw_exception_for_null_stream()
        {
            // arrange

            // act
            Assert.Throws<ArgumentNullException>(() => NbtDocument.IsNbtDocument((string) null));

            // assert
        }

        [Fact]
        public void IsNbtDocument_should_throw_exception_if_file_is_missing()
        {
            // arrange
            var fileName = Guid.NewGuid().ToString();

            // act
            Assert.Throws<FileNotFoundException>(() => NbtDocument.IsNbtDocument(fileName));

            // assert
        }

        [Fact]
        public void Load_should_throw_exception_for_invalid_file()
        {
            // arrange
            var fileName = BadFileName;

            var target = new NbtDocument();

            // act
            Assert.Throws<InvalidDataException>(() => target.Load(fileName));
        }

        [Fact]
        public void Load_should_throw_exception_for_missing_file()
        {
            // arrange
            var fileName = Guid.NewGuid().ToString("N");

            var target = new NbtDocument();

            // act
            Assert.Throws<FileNotFoundException>(() => target.Load(fileName));
        }

        [Fact]
        public void Load_should_throw_exception_for_null_filename()
        {
            // arrange
            var target = new NbtDocument();

            // act
            Assert.Throws<ArgumentNullException>(() => target.Load((string) null));
        }

        [Fact]
        public void Load_should_throw_exception_for_null_stream()
        {
            // arrange
            var target = new NbtDocument();

            // act
            Assert.Throws<ArgumentNullException>(() => target.Load((Stream) null));
        }

        [Fact]
        public void Load_updates_filename_property()
        {
            // arrange
            var expected = ComplexDataFileName;

            var target = new NbtDocument();

            // act
            target.Load(expected);

            // assert
            var actual = target.FileName;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LoadDocument_loads_data_from_stream()
        {
            // arrange
            NbtDocument actual;
            var fileName = ComplexDataFileName;
            var expected = new NbtDocument(CreateComplexData());

            // act
            using (Stream stream = File.OpenRead(fileName))
            {
                actual = NbtDocument.LoadDocument(stream);
            }

            // assert
            NbtAssert.Equal(expected, actual);
        }

        [Fact]
        public void LoadTest()
        {
            // arrange

            var fileName = ComplexDataFileName;
            var expected = new NbtDocument(CreateComplexData());
            var target = new NbtDocument
            {
                FileName = fileName
            };

            // act
            target.Load();

            // assert
            NbtAssert.Equal(expected, target);
        }

        [Fact]
        public void LoadWithFileTest()
        {
            // arrange
            var fileName = ComplexDataFileName;
            var expected = new NbtDocument(CreateComplexData());
            var target = new NbtDocument();

            // act
            target.Load(fileName);

            // assert
            NbtAssert.Equal(expected, target);
        }

        [Fact]
        public void Query_returns_tag()
        {
            // arrange
            var target = new NbtDocument(CreateComplexData());
            var expected =
                ((TagCompound) ((TagList) target.DocumentRoot["listTest (compound)"]).Value[1])["created-on"];

            // act
            var actual = target.Query(@"listTest (compound)\1\created-on");

            // assert
            Assert.Same(expected, actual);
        }

        [Fact]
        public void Query_returns_typed_tag()
        {
            // arrange
            var target = new NbtDocument(CreateComplexData());
            var expected =
                (TagLong) ((TagCompound) ((TagList) target.DocumentRoot["listTest (compound)"]).Value[1])["created-on"];

            // act
            var actual = target.Query<TagLong>(@"listTest (compound)\1\created-on");

            // assert
            Assert.Same(expected, actual);
        }

        [Fact]
        public void Save_throws_exception_if_filename_is_empty()
        {
            // arrange
            var target = new NbtDocument();

            // act
            var e = Assert.Throws<ArgumentNullException>(() => target.Save(string.Empty));
            Assert.Equal("Value cannot be null.\r\nParameter name: fileName", e.Message);
        }

        [Fact]
        public void Save_throws_exception_if_filename_is_null()
        {
            // arrange
            var target = new NbtDocument();

            // act
            var e = Assert.Throws<ArgumentNullException>(() => target.Save((string) null));
            Assert.Equal("Value cannot be null.\r\nParameter name: fileName", e.Message);
        }

        [Fact]
        public void Save_throws_exception_if_stream_is_null()
        {
            // arrange

            var target = new NbtDocument();

            // act
            var e = Assert.Throws<ArgumentNullException>(() => target.Save((Stream) null));
            Assert.Equal("Value cannot be null.\r\nParameter name: stream", e.Message);
        }

        [Fact]
        public void Save_updates_filename_property()
        {
            // arrange
            var expected = GetWorkFile();
            var target = new NbtDocument(CreateComplexData());

            // act
            try
            {
                target.Save(expected);
            }
            finally
            {
                DeleteFile(expected);
            }

            // assert
            var actual = target.FileName;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SaveTest()
        {
            // arrange
            NbtDocument target;

            var fileName = GetWorkFile();
            var expected = new NbtDocument(CreateComplexData())
            {
                FileName = fileName
            };

            // act
            try
            {
                expected.Save();
                target = NbtDocument.LoadDocument(fileName);
            }
            finally
            {
                DeleteFile(fileName);
            }

            // assert
            NbtAssert.Equal(expected, target);
        }

        [Fact]
        public void SaveWithFileTest()
        {
            // arrange
            NbtDocument target;

            var fileName = GetWorkFile();
            var expected = new NbtDocument(CreateComplexData());

            // act
            try
            {
                expected.Save(fileName);
                target = NbtDocument.LoadDocument(fileName);
            }
            finally
            {
                DeleteFile(fileName);
            }

            // assert
            NbtAssert.Equal(expected, target);
        }

        [Fact]
        public void ToString_returns_tag_hieararchy()
        {
            // arrange
            var target = new NbtDocument(CreateComplexData());
            var sb = new StringBuilder();
            sb.AppendLine("compound:Level");
            sb.AppendLine("  long:longTest [9223372036854775807]");
            sb.AppendLine("  short:shortTest [32767]");
            sb.AppendLine("  string:stringTest [HELLO WORLD THIS IS A TEST STRING ÅÄÖ!]");
            sb.AppendLine("  float:floatTest [0.4982315]");
            sb.AppendLine("  int:intTest [2147483647]");
            sb.AppendLine("  compound:nested compound test");
            sb.AppendLine("    compound:ham");
            sb.AppendLine("      string:name [Hampus]");
            sb.AppendLine("      float:value [0.75]");
            sb.AppendLine("    compound:egg");
            sb.AppendLine("      string:name [Eggbert]");
            sb.AppendLine("      float:value [0.5]");
            sb.AppendLine("  list:listTest (long)");
            sb.AppendLine("    long#0 [11]");
            sb.AppendLine("    long#1 [12]");
            sb.AppendLine("    long#2 [13]");
            sb.AppendLine("    long#3 [14]");
            sb.AppendLine("    long#4 [15]");
            sb.AppendLine("  list:listTest (compound)");
            sb.AppendLine("    compound#0");
            sb.AppendLine("      string:name [Compound tag #0]");
            sb.AppendLine("      long:created-on [1264099775885]");
            sb.AppendLine("    compound#1");
            sb.AppendLine("      string:name [Compound tag #1]");
            sb.AppendLine("      long:created-on [1264099775885]");
            sb.AppendLine("  byte:byteTest [127]");
            sb.AppendLine(
                "  bytearray:byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...)) [00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30, 00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30, 00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30, 00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30, 00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30, 00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30, 00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30, 00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30, 00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30, 00, 3E, 22, 10, 08, 0A, 16, 2C, 4C, 12, 46, 20, 04, 56, 4E, 50, 5C, 0E, 2E, 58, 28, 02, 4A, 38, 30, 32, 3E, 54, 10, 3A, 0A, 48, 2C, 1A, 12, 14, 20, 36, 56, 1C, 50, 2A, 0E, 60, 58, 5A, 02, 18, 38, 62, 32, 0C, 54, 42, 3A, 3C, 48, 5E, 1A, 44, 14, 52, 36, 24, 1C, 1E, 2A, 40, 60, 26, 5A, 34, 18, 06, 62, 00, 0C, 22, 42, 08, 3C, 16, 5E, 4C, 44, 46, 52, 04, 24, 4E, 1E, 5C, 40, 2E, 26, 28, 34, 4A, 06, 30]");
            sb.AppendLine("  double:doubleTest [0.493128713218231]");

            var expected = sb.ToString();

            // act
            var actual = target.ToString();

            // assert
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}