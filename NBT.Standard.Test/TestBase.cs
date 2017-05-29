using System;
using System.IO;
using NBT.Serialization;

namespace NBT.Test
{
    public class TestBase
    {
        #region Constructors

        protected TestBase()
        {
            BasePath = Directory.GetParent(".").Parent.Parent.FullName;
        }

        #endregion

        #region Properties

        public string BasePath { get; set; }

        protected string AnvilRegionFileName => Path.Combine(DataPath, "r.0.0.mca");

        protected string LevelDatFileName => Path.Combine(DataPath, "level.dat");

        protected string BadFileName => Path.Combine(DataPath, "badfile.txt");

        protected string ComplexDataFileName => Path.Combine(DataPath, "bigtest.nbt");

        protected string ComplexXmlDataFileName => Path.Combine(DataPath, "complextest.xml");

        protected string ComplexXmlWithoutWhitespaceDataFileName => Path.Combine(DataPath, "complextest-no-ws.xml");

        protected string DataPath => Path.Combine(BasePath, "data");

        protected string DeflateComplexDataFileName => Path.Combine(DataPath, "complextest.def");

        protected string SimpleDataFileName => Path.Combine(DataPath, "test.nbt");

        protected string UncompressedComplexDataFileName => Path.Combine(DataPath, "bigtest.raw");

        #endregion

        #region Methods

        protected TagCompound CreateComplexData()
        {
            var root = new TagCompound
            {
                Name = "Level"
            };
            root.Value.Add("longTest", 9223372036854775807);
            root.Value.Add("shortTest", (short) 32767);
            root.Value.Add("stringTest", "HELLO WORLD THIS IS A TEST STRING ÅÄÖ!");
            root.Value.Add("floatTest", (float) 0.498231471);
            root.Value.Add("intTest", 2147483647);

            var compound = (TagCompound) root.Value.Add("nested compound test", TagType.Compound);
            var child = (TagCompound) compound.Value.Add("ham", TagType.Compound);
            child.Value.Add("name", "Hampus");
            child.Value.Add("value", (float) 0.75);
            child = (TagCompound) compound.Value.Add("egg", TagType.Compound);
            child.Value.Add("name", "Eggbert");
            child.Value.Add("value", (float) 0.5);

            var list = (TagList) root.Value.Add("listTest (long)", TagType.List, TagType.Long);
            list.Value.Add((long) 11);
            list.Value.Add((long) 12);
            list.Value.Add((long) 13);
            list.Value.Add((long) 14);
            list.Value.Add((long) 15);

            list = (TagList) root.Value.Add("listTest (compound)", TagType.List, TagType.Compound);
            child = (TagCompound) list.Value.Add(TagType.Compound);
            child.Value.Add("name", "Compound tag #0");
            child.Value.Add("created-on", 1264099775885);
            child = (TagCompound) list.Value.Add(TagType.Compound);
            child.Value.Add("name", "Compound tag #1");
            child.Value.Add("created-on", 1264099775885);

            root.Value.Add("byteTest", (byte) 127);
            root.Value.Add(
                "byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))",
                ComplexData.SampleByteArray);
            root.Value.Add("doubleTest", 0.49312871321823148);

            return root;
        }

        protected TagCompound CreateSimpleNesting()
        {
            var root = new TagCompound("project");
            var list = new TagList("slices", TagType.Compound);
            var compound = new TagCompound();
            compound.Value.Add(new TagCompound("location"));
            list.Value.Add(compound);
            root.Value.Add(list);
            list = new TagList("regions", TagType.Compound);
            list.Value.Add(new TagCompound());
            list.Value.Add(new TagCompound());
            root.Value.Add(list);

            return root;
        }

        protected void DeleteFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
            }
        }

        protected TagCompound GetComplexData()
        {
            return NbtDocument.LoadDocument(ComplexDataFileName).DocumentRoot;
        }

        protected TagCompound GetSimpleData()
        {
            return NbtDocument.LoadDocument(SimpleDataFileName).DocumentRoot;
        }

        protected string GetWorkFile()
        {
            var fileName = string.Concat(Guid.NewGuid().ToString("N"), ".dat");
            var path = BasePath;

            return Path.Combine(path, fileName);
        }

        protected void WriteDocumentTest<T, T2>(Func<Stream, T> createWriter, Func<Stream, T2> createReader)
            where T : TagWriter where T2 : TagReader
        {
            // arrange
            Stream stream = new MemoryStream();

            var expected = CreateComplexData();

            TagWriter target = createWriter(stream);

            // act
            target.WriteStartDocument();
            target.WriteTag(expected);
            target.WriteEndDocument();

            // assert
            stream.Seek(0, SeekOrigin.Begin);
            TagReader reader = createReader(stream);
            var actual = reader.ReadDocument();
            NbtAssert.Equal(expected, actual);
        }

        protected void WriteTest<T, T2>(Func<Stream, T> createWriter, Func<Stream, T2> createReader)
            where T : TagWriter where T2 : TagReader
        {
            // arrange
            Stream stream = new MemoryStream();

            var expected = CreateComplexData();

            TagWriter target = createWriter(stream);

            // act
            target.WriteStartDocument();
            target.WriteStartTag("Level", TagType.Compound);
            target.WriteTag("longTest", 9223372036854775807);
            target.WriteTag("shortTest", (short) 32767);
            target.WriteTag("stringTest", "HELLO WORLD THIS IS A TEST STRING ÅÄÖ!");
            target.WriteTag("floatTest", (float) 0.498231471);
            target.WriteTag("intTest", 2147483647);
            target.WriteStartTag("nested compound test", TagType.Compound);
            target.WriteStartTag("ham", TagType.Compound);
            target.WriteTag("name", "Hampus");
            target.WriteTag("value", 0.75F);
            target.WriteEndTag();
            target.WriteStartTag("egg", TagType.Compound);
            target.WriteTag("name", "Eggbert");
            target.WriteTag("value", 0.5F);
            target.WriteEndTag();
            target.WriteEndTag();
            target.WriteStartTag("listTest (long)", TagType.List, TagType.Long, 5);
            target.WriteTag((long) 11);
            target.WriteTag((long) 12);
            target.WriteTag((long) 13);
            target.WriteTag((long) 14);
            target.WriteTag((long) 15);
            target.WriteEndTag();
            target.WriteStartTag("listTest (compound)", TagType.List, TagType.Compound, 2);
            target.WriteStartTag(TagType.Compound);
            target.WriteTag("name", "Compound tag #0");
            target.WriteTag("created-on", 1264099775885);
            target.WriteEndTag();
            target.WriteStartTag(TagType.Compound);
            target.WriteTag("name", "Compound tag #1");
            target.WriteTag("created-on", 1264099775885);
            target.WriteEndTag();
            target.WriteEndTag();
            target.WriteTag("byteTest", (byte) 127);
            target.WriteTag(
                "byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))",
                ComplexData.SampleByteArray);
            target.WriteTag("doubleTest", 0.49312871321823148);
            target.WriteEndTag();
            target.WriteEndDocument();

            // assert
            stream.Seek(0, SeekOrigin.Begin);
            TagReader reader = createReader(stream);
            var actual = reader.ReadDocument();
            NbtAssert.Equal(expected, actual);
        }

        #endregion
    }
}