using System;
using System.IO;
using System.IO.Compression;
using NBT.Serialization;
using Xunit;

namespace NBT.Test
{
    public class LevelDatTests : TestBase
    {
        #region  Tests

        [Fact]
        public void TestLevelDat()
        {
            var filename = LevelDatFileName;
            var input = File.OpenRead(filename);

            var document = NbtDocument.LoadDocument(input);

            var tag = document.DocumentRoot;
            Assert.NotNull(tag);

            Assert.Equal(TagType.Compound, tag.GetTag("Data").Type);
            var dataTag = tag.GetCompound("Data");

            var aTag = dataTag.GetTag("RandomSeed");
            Assert.Equal(TagType.Long, aTag.Type);
            var randomSeedTag = aTag as TagLong;
            Assert.Equal(4339722475394340739, randomSeedTag.Value);

            aTag = dataTag.GetTag("LevelName");
            Assert.Equal(TagType.String, aTag.Type);
            var levelNameTag = aTag as TagString;
            Assert.Equal("New World", levelNameTag.Value);

            aTag = dataTag.GetTag("Player");
            Assert.Equal(TagType.Compound, aTag.Type);
            var playerTag = aTag as TagCompound;
            Assert.NotNull(playerTag);

            aTag = playerTag.GetTag("UUIDLeast");
            Assert.Equal(TagType.Long, aTag.Type);
            var uuidLeastTag = aTag as TagLong;
            Assert.Equal(-6361924693575993529, uuidLeastTag.Value);

            aTag = playerTag.GetTag("EnderItems");
            Assert.Equal(TagType.List, aTag.Type);
            var enderItemTag = aTag as TagList;
            Assert.Equal(0, enderItemTag.Count);
        }

        #endregion
    }
}