using System;
using System.Linq;
using Xunit;

namespace NBT.Test
{
    public partial class TagCollectionTests : TestBase
    {
        #region  Tests

        [Fact]
        public void Add_sets_limit_type()
        {
            // arrange
            var target = new TagCollection();
            const TagType expected = TagType.Byte;

            // act
            target.Add((byte) 127);

            // assert
            var actual = target.LimitType;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Add_sets_parent()
        {
            // arrange
            Tag actual = new TagByte(56);
            var owner = new TagList();
            var target = owner.Value;

            // act
            target.Add(actual);

            // assert
            Assert.Same(owner, actual.Parent);
        }

        [Fact]
        public void Add_throws_exception_for_named_tags()
        {
            // arrange
            var target = new TagCollection(TagType.Byte);

            // act
            var e = Assert.Throws<ArgumentException>(() => target.Add(new TagByte("alpha", 120)));
            Assert.Equal($"Only unnamed tags are supported.{Environment.NewLine}Parameter name: item", e.Message);
        }

        [Fact]
        public void Add_throws_exception_for_tags_not_matching_list_type()
        {
            // arrange
            var target = new TagCollection(TagType.Byte);

            // act
            var e = Assert.Throws<ArgumentException>(() => target.Add(int.MaxValue));
            Assert.Equal(
                $"Only items of type Byte can be added to this collection.{Environment.NewLine}Parameter name: item",
                e.Message);
        }

        [Fact]
        public void Add_throws_exception_for_unsupported_data_type()
        {
            // arrange
            var target = new TagCollection();

            // act
            var e = Assert.Throws<ArgumentException>(() => target.Add(TimeSpan.MinValue));
            Assert.Equal($"Invalid value type.{Environment.NewLine}Parameter name: value", e.Message);
        }

        [Fact]
        public void AddRange_adds_values()
        {
            // arrange
            var target = new TagCollection();
            var expected = new object[]
            {
                8,
                16,
                32
            };

            // act
            target.AddRange(expected);

            // assert
            Assert.Equal(expected.Length, target.Count);
            Assert.Equal(expected, target.Select(t => t.GetValue()).ToArray());
        }

        [Fact]
        public void Clear_removes_parents()
        {
            // arrange
            var actual1 = new TagByte(56);
            var actual2 = new TagByte(156);
            var owner = new TagList();
            var target = owner.Value;

            target.Add(actual1);
            target.Add(actual2);

            // act
            target.Clear();

            // assert
            Assert.Null(actual1.Parent);
            Assert.Null(actual2.Parent);
        }

        [Fact]
        public void Constructor_sets_default_limit_type()
        {
            // arrange
            const TagType expected = TagType.None;

            // act
            var target = new TagCollection();

            // assert
            var actual = target.LimitType;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Constructor_sets_limit_type()
        {
            // arrange
            const TagType expected = TagType.Byte;

            // act
            var target = new TagCollection(expected);

            // assert
            var actual = target.LimitType;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Indexer_sets_parent()
        {
            // arrange
            Tag actual = new TagByte(56);

            var owner = new TagList();
            var target = owner.Value;

            target.Add(byte.MaxValue);

            // act
            target[0] = actual;

            // assert
            Assert.Same(owner, actual.Parent);
        }

        [Fact]
        public void Indexer_throws_exception_for_tags_not_matching_list_type()
        {
            // arrange
            // ReSharper disable once CollectionNeverQueried.Local
            var target = new TagCollection(TagType.Byte)
            {
                byte.MaxValue
            };

            // act
            var e = Assert.Throws<ArgumentException>(() => target[0] = new TagInt());
            Assert.Equal(
                $"Only items of type Byte can be added to this collection.{Environment.NewLine}Parameter name: item",
                e.Message);
        }

        [Fact]
        public void Remove_clears_parent()
        {
            // arrange
            var owner = new TagList();
            var target = owner.Value;

            Tag actual = target.Add((byte) 56);

            // act
            target.Remove(actual);

            // assert
            Assert.Null(actual.Parent);
        }

        #endregion
    }
}