using System;
using Xunit;

namespace NBT.Test
{
    public partial class TagFactoryTests
    {
        #region  Tests

        [Fact]
        public void CreateTag_creates_list()
        {
            // arrange
            const TagType type = TagType.List;
            const TagType listType = TagType.Byte;

            // act
            var actual = TagFactory.CreateTag(type, listType);

            // assert
            Assert.NotNull(actual);
            Assert.Equal(listType, actual.ListType);
        }

        [Fact]
        public void CreateTag_throws_exception_for_invalid_type()
        {
            // arrange
            const TagType type = (TagType) (-1);

            // act
            var e = Assert.Throws<ArgumentException>(() => TagFactory.CreateTag(type));
            Assert.Equal($"Unrecognized or unsupported tag type.{Environment.NewLine}Parameter name: tagType",
                e.Message);
        }

        [Fact]
        public void CreateTag_with_list_type_for_non_list_throws_exception()
        {
            // arrange
            const TagType type = TagType.Byte;
            const TagType listType = TagType.ByteArray;

            // act
            var e = Assert.Throws<ArgumentException>(() => TagFactory.CreateTag(string.Empty, type, listType));
            Assert.Equal($"Only lists can have a list type.{Environment.NewLine}Parameter name: listType", e.Message);
        }

        [Fact]
        public void CreateTag_with_value_throws_exception_for_invalid_type()
        {
            // arrange
            const TagType type = (TagType) (-1);

            // act
            var e = Assert.Throws<ArgumentException>(() => TagFactory.CreateTag(string.Empty, type, 13));
            Assert.Equal($"Unrecognized or unsupported tag type.{Environment.NewLine}Parameter name: tagType",
                e.Message);
        }

        #endregion
    }
}