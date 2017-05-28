using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using NBT.Serialization;
using Xunit;

namespace NBT.Test
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class AnvilRegionTests : TestBase
    {
        #region  Tests

        [Fact]
        public void TestAnvilRegion()
        {
            var filename = AnvilRegionFileName;
            var input = File.OpenRead(filename);
            var locations = new int[1024];
            var buffer = new byte[4096];
            input.Read(buffer, 0, 4096);
            for (var i = 0; i < 1024; i++)
            {
                locations[i] = BitConverter.ToInt32(buffer, i * 4);
            }

            var timestamps = new int[1024];
            input.Read(buffer, 0, 4096);
            for (var i = 0; i < 1024; i++)
            {
                timestamps[i] = BitConverter.ToInt32(buffer, i * 4);
            }

            input.Read(buffer, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                BitHelper.SwapBytes(buffer, 0, 4);
            }
            var sizeOfChunkData = BitConverter.ToInt32(buffer, 0) - 1;

            var compressionType = input.ReadByte();
            buffer = new byte[sizeOfChunkData];
            input.Read(buffer, 0, sizeOfChunkData);

            Stream inputStream = null;

            if (compressionType == 1)
            {
                inputStream = new GZipStream(new MemoryStream(buffer), CompressionMode.Decompress);
            }
            else if (compressionType == 2)
            {
                inputStream = new DeflateStream(new MemoryStream(buffer, 2, buffer.Length - 6),
                    CompressionMode.Decompress);
            }

            TagReader reader;
            reader = new BinaryTagReader(inputStream);
            var tag = (TagCompound) reader.ReadTag();

            Assert.NotNull(tag);

            Assert.Equal(TagType.Compound, tag.GetTag("Level").Type);
            var levelTag = tag.GetCompound("Level");

            var aTag = levelTag.GetTag("Entities");
            Assert.Equal(TagType.List, aTag.Type);
            var entitiesTag = aTag as TagList;
            Assert.Equal(0, entitiesTag.Value.Count);

            aTag = levelTag.GetTag("Biomes");
            Assert.Equal(TagType.ByteArray, aTag.Type);
            var biomesTag = aTag as TagByteArray;
            Assert.Equal(256, biomesTag.Value.Length);

            aTag = levelTag.GetTag("LastUpdate");
            Assert.Equal(TagType.Long, aTag.Type);
            var lastUpdateTag = aTag as TagLong;
            Assert.Equal(2861877, lastUpdateTag.Value);

            aTag = levelTag.GetTag("xPos");
            Assert.Equal(TagType.Int, aTag.Type);
            var xPosTag = aTag as TagInt;
            Assert.Equal(10, xPosTag.Value);

            aTag = levelTag.GetTag("zPos");
            Assert.Equal(TagType.Int, aTag.Type);
            var zPosTag = aTag as TagInt;
            Assert.Equal(0, zPosTag.Value);

            aTag = levelTag.GetTag("TileEntities");
            Assert.Equal(TagType.List, aTag.Type);
            var tileEntitiesTag = aTag as TagList;
            Assert.Equal(0, tileEntitiesTag.Value.Count);

            aTag = levelTag.GetTag("TerrainPopulated");
            Assert.Equal(TagType.Byte, aTag.Type);
            var terrainPopulatedTag = aTag as TagByte;
            Assert.Equal(1, terrainPopulatedTag.Value);

            aTag = levelTag.GetTag("HeightMap");
            Assert.Equal(TagType.IntArray, aTag.Type);
            var heightmapTag = aTag as TagIntArray;
            Assert.Equal(256, heightmapTag.Value.Length);

            aTag = levelTag.GetTag("Sections");
            Assert.Equal(TagType.List, aTag.Type);
            var sectionsTag = aTag as TagList;
            Assert.Equal(4, sectionsTag.Value.Count);

            var section0 = sectionsTag.Value[0] as TagCompound;
            Assert.NotNull(section0);
            var section0Data = section0.GetByteArray("Data");
            Assert.NotNull(section0Data);
            Assert.Equal(2048, section0Data.Value.Length);
            var section0SkyLight = section0.GetByteArray("SkyLight");
            Assert.NotNull(section0SkyLight);
            Assert.Equal(2048, section0SkyLight.Value.Length);
            var section0BlockLight = section0.GetByteArray("BlockLight");
            Assert.NotNull(section0BlockLight);
            Assert.Equal(2048, section0BlockLight.Value.Length);
            var section0Y = section0.GetByte("Y");
            Assert.NotNull(section0Y);
            Assert.Equal(0, section0Y.Value);
            var section0Blocks = section0.GetByteArray("Blocks");
            Assert.NotNull(section0Blocks);
            Assert.Equal(4096, section0Blocks.Value.Length);

            var section1 = sectionsTag.Value[1] as TagCompound;
            Assert.NotNull(section1);
            var section1Data = section1.GetByteArray("Data");
            Assert.NotNull(section1Data);
            Assert.Equal(2048, section1Data.Value.Length);
            var section1SkyLight = section1.GetByteArray("SkyLight");
            Assert.NotNull(section1SkyLight);
            Assert.Equal(2048, section1SkyLight.Value.Length);
            var section1BlockLight = section1.GetByteArray("BlockLight");
            Assert.NotNull(section1BlockLight);
            Assert.Equal(2048, section1BlockLight.Value.Length);
            var section1Y = section1.GetByte("Y");
            Assert.NotNull(section1Y);
            Assert.Equal(1, section1Y.Value);
            var section1Blocks = section1.GetByteArray("Blocks");
            Assert.NotNull(section1Blocks);
            Assert.Equal(4096, section1Blocks.Value.Length);

            var section2 = sectionsTag.Value[2] as TagCompound;
            Assert.NotNull(section2);
            var section2Data = section2.GetByteArray("Data");
            Assert.NotNull(section2Data);
            Assert.Equal(2048, section2Data.Value.Length);
            var section2SkyLight = section2.GetByteArray("SkyLight");
            Assert.NotNull(section2SkyLight);
            Assert.Equal(2048, section2SkyLight.Value.Length);
            var section2BlockLight = section2.GetByteArray("BlockLight");
            Assert.NotNull(section2BlockLight);
            Assert.Equal(2048, section2BlockLight.Value.Length);
            var section2Y = section2.GetByte("Y");
            Assert.NotNull(section2Y);
            Assert.Equal(2, section2Y.Value);
            var section2Blocks = section2.GetByteArray("Blocks");
            Assert.NotNull(section2Blocks);
            Assert.Equal(4096, section2Blocks.Value.Length);

            var section3 = sectionsTag.Value[3] as TagCompound;
            Assert.NotNull(section3);
            var section3Data = section3.GetByteArray("Data");
            Assert.NotNull(section3Data);
            Assert.Equal(2048, section3Data.Value.Length);
            var section3SkyLight = section3.GetByteArray("SkyLight");
            Assert.NotNull(section3SkyLight);
            Assert.Equal(2048, section3SkyLight.Value.Length);
            var section3BlockLight = section3.GetByteArray("BlockLight");
            Assert.NotNull(section3BlockLight);
            Assert.Equal(2048, section3BlockLight.Value.Length);
            var section3Y = section3.GetByte("Y");
            Assert.NotNull(section3Y);
            Assert.Equal(3, section3Y.Value);
            var section3Blocks = section3.GetByteArray("Blocks");
            Assert.NotNull(section3Blocks);
            Assert.Equal(4096, section3Blocks.Value.Length);
        }

        #endregion
    }
}