using System.IO;
using System.IO.Compression;
using System.Linq;
using NBT.Serialization;
using Xunit;

namespace NBT.Test
{
    public class TagTests : TestBase
    {
        #region  Tests

        [Fact]
        public void Attempting_to_set_name_to_null_sets_empty_string()
        {
            // arrange
            Tag target = new TagByte("alpha")
            {
                // act
                Name = null
            };

            // assert
            Assert.Empty(target.Name);
        }

        [Fact]
        public void Equals_returns_false_for_null_object()
        {
            // arrange
            var target = new TagByte();

            // act
            var actual = target.Equals((object) null);

            // assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_returns_true_for_same_reference()
        {
            // arrange
            var target = new TagByte();

            // act
            var actual = target.Equals((object) target);

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_returns_true_for_same_tag()
        {
            // arrange
            var target = new TagByte(127);
            var other = new TagByte(127);

            // act
            var actual = target.Equals((object) other);

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void Flatten_returns_tag_and_all_descendants()
        {
            // arrange
            var target = CreateComplexData();
            const int expectedCount = 29;
            var expectedNames = new[]
            {
                "Level",
                "longTest",
                "shortTest",
                "stringTest",
                "floatTest",
                "intTest",
                "nested compound test",
                "ham",
                "name",
                "value",
                "egg",
                "listTest (long)",
                "",
                "listTest (compound)",
                "created-on",
                "byteTest",
                "byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))",
                "doubleTest"
            };

            // act
            var tags = target.Flatten();
            var actualCount = tags.Length;
            var actualNames = tags.Select(t => t.Name).Distinct().ToArray();

            // assert
            Assert.Equal(expectedCount, actualCount);
            Assert.Equal(expectedNames, actualNames);
        }

        [Fact]
        public void Flattern_returns_tab()
        {
            // arrange
            var target = new TagByte();

            // act
            var actual = target.Flatten();

            // assert
            Assert.Equal(1, actual.Length);
            Assert.Same(target, actual[0]);
        }

        [Fact]
        public void FullPathTest()
        {
            // 
            var data = CreateComplexData();
            const string expected = @"Level\listTest (compound)\0\name";
            var target = data.Query(@"listTest (compound)\0\name");

            // act
            var actual = target.FullPath;

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetAncestorsTest()
        {
            // arrange
            var data = CreateComplexData();
            var target = data.Query(@"listTest (compound)\0\name");
            var expected = new[]
            {
                data,
                data.Value["listTest (compound)"],
                data.Query(@"listTest (compound)\0")
            };

            // act
            var actual = target.GetAncestors();

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadExceptionTest()
        {
            // arrange
            var stream = new MemoryStream();
            stream.WriteByte(255);
            stream.Seek(0, SeekOrigin.Begin);
            TagReader reader = new BinaryTagReader(stream);

            // act
            var e = Assert.Throws<InvalidDataException>(() => reader.ReadTag());
            Assert.Equal("Unrecognized tag type: 255.", e.Message);
        }

        [Fact]
        public void TestLoadComplexNbt()
        {
            var tag = CreateComplexData();

            Assert.NotNull(tag);
            Assert.IsType<TagCompound>(tag);
            var level = tag;
            Assert.Equal("Level", level.Name);

            var shortTest = level.GetShort("shortTest");
            Assert.NotNull(shortTest);
            Assert.Equal("shortTest", shortTest.Name);
            Assert.Equal(32767, shortTest.Value);

            var longTest = level.GetLong("longTest");
            Assert.NotNull(longTest);
            Assert.Equal("longTest", longTest.Name);
            Assert.Equal(9223372036854775807, longTest.Value);

            var floatTest = level.GetFloat("floatTest");
            Assert.NotNull(floatTest);
            Assert.Equal("floatTest", floatTest.Name);
            Assert.Equal(0.49823147f, floatTest.Value);

            var stringTest = level.GetString("stringTest");
            Assert.NotNull(stringTest);
            Assert.Equal("stringTest", stringTest.Name);
            Assert.Equal("HELLO WORLD THIS IS A TEST STRING ÅÄÖ!", stringTest.Value);

            var intTest = level.GetInt("intTest");
            Assert.NotNull(intTest);
            Assert.Equal("intTest", intTest.Name);
            Assert.Equal(2147483647, intTest.Value);

            var nestedCompoundTest = level.GetCompound("nested compound test");
            Assert.NotNull(nestedCompoundTest);
            Assert.Equal("nested compound test", nestedCompoundTest.Name);

            var ham = nestedCompoundTest.GetCompound("ham");
            Assert.NotNull(ham);
            Assert.Equal("ham", ham.Name);

            var hamName = ham.GetString("name");
            Assert.NotNull(hamName);
            Assert.Equal("name", hamName.Name);
            Assert.Equal("Hampus", hamName.Value);

            var hamValue = ham.GetFloat("value");
            Assert.NotNull(hamValue);
            Assert.Equal("value", hamValue.Name);
            Assert.Equal(0.75f, hamValue.Value);

            var egg = nestedCompoundTest.GetCompound("egg");
            Assert.NotNull(egg);
            Assert.Equal("egg", egg.Name);

            var eggName = egg.GetString("name");
            Assert.NotNull(eggName);
            Assert.Equal("name", eggName.Name);
            Assert.Equal("Eggbert", eggName.Value);

            var eggValue = egg.GetFloat("value");
            Assert.NotNull(eggValue);
            Assert.Equal("value", eggValue.Name);
            Assert.Equal(0.5f, eggValue.Value);

            var byteTest = level.GetByte("byteTest");
            Assert.NotNull(byteTest);
            Assert.Equal("byteTest", byteTest.Name);
            Assert.Equal(0x7f, byteTest.Value);

            var doubleTest = level.GetDouble("doubleTest");
            Assert.NotNull(doubleTest);
            Assert.Equal("doubleTest", doubleTest.Name);
            Assert.Equal(0.4931287132182315, doubleTest.Value);

            var listTestLong = level.GetList("listTest (long)");
            Assert.NotNull(listTestLong);
            Assert.Equal("listTest (long)", listTestLong.Name);
            Assert.NotNull(listTestLong.Value);
            Assert.Equal(5, listTestLong.Value.Count);
            Assert.Equal(11, ((TagLong) listTestLong.Value[0]).Value);
            Assert.Equal(12, ((TagLong) listTestLong.Value[1]).Value);
            Assert.Equal(13, ((TagLong) listTestLong.Value[2]).Value);
            Assert.Equal(14, ((TagLong) listTestLong.Value[3]).Value);
            Assert.Equal(15, ((TagLong) listTestLong.Value[4]).Value);

            var listTestCompound = level.GetList("listTest (compound)");
            Assert.NotNull(listTestCompound);
            Assert.Equal("listTest (compound)", listTestCompound.Name);
            Assert.NotNull(listTestCompound.Value);
            Assert.Equal(2, listTestCompound.Value.Count);

            var listTestCompoundTag0 = listTestCompound.Value[0] as TagCompound;
            Assert.NotNull(listTestCompoundTag0);

            var listTestCompoundTag0Name = listTestCompoundTag0.GetString("name");
            Assert.NotNull(listTestCompoundTag0Name);
            Assert.Equal("name", listTestCompoundTag0Name.Name);
            Assert.Equal("Compound tag #0", listTestCompoundTag0Name.Value);

            var listTestCompoundTag0CreatedOn = listTestCompoundTag0.GetLong("created-on");
            Assert.NotNull(listTestCompoundTag0CreatedOn);
            Assert.Equal("created-on", listTestCompoundTag0CreatedOn.Name);
            Assert.Equal(1264099775885, listTestCompoundTag0CreatedOn.Value);

            var listTestCompoundTag1 = listTestCompound.Value[1] as TagCompound;
            Assert.NotNull(listTestCompoundTag1);

            var listTestCompoundTag1Name = listTestCompoundTag1.GetString("name");
            Assert.NotNull(listTestCompoundTag1Name);
            Assert.Equal("name", listTestCompoundTag1Name.Name);
            Assert.Equal("Compound tag #1", listTestCompoundTag1Name.Value);

            var listTestCompoundTag1CreatedOn = listTestCompoundTag1.GetLong("created-on");
            Assert.NotNull(listTestCompoundTag1CreatedOn);
            Assert.Equal("created-on", listTestCompoundTag1CreatedOn.Name);
            Assert.Equal(1264099775885, listTestCompoundTag1CreatedOn.Value);

            var byteArrayTest =
                level.GetByteArray(
                    "byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))");
            Assert.NotNull(byteArrayTest);
            Assert.Equal(
                "byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))",
                byteArrayTest.Name);
            Assert.NotNull(byteArrayTest.Value);
            Assert.Equal(1000, byteArrayTest.Value.Length);
        }

        [Fact]
        public void TestLoadSimpleNbt()
        {
            var tag = GetSimpleData();

            Assert.NotNull(tag);
            Assert.IsType<TagCompound>(tag);
            var root = tag;
            Assert.Equal("hello world", root.Name);

            var tagStr = root.GetString("name");
            Assert.Equal("name", tagStr.Name);

            Assert.Equal("Bananrama", tagStr.Value);
        }

        [Fact]
        public void TestReadNonExistantTagFromCompound()
        {
            var newTag = new TagCompound();

            var aTag = newTag.GetTag("nope");

            Assert.Null(aTag);

            var fileTag = GetSimpleData();

            aTag = fileTag.GetTag("Entities");

            Assert.Null(aTag);
        }

        #endregion

        #region Test Helpers

        /// <summary>
        ///   Decompresses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        private byte[] Decompress(Stream stream)
        {
            byte[] result;

            using (var decompressStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                var bufferSize = 4096;
                var buffer = new byte[bufferSize];

                using (var memory = new MemoryStream())
                {
                    int count;
                    do
                    {
                        count = decompressStream.Read(buffer, 0, bufferSize);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    } while (count > 0);

                    result = memory.ToArray();
                }
            }

            return result;
        }

        #endregion
    }
}