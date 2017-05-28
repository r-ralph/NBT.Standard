using System;
using Xunit;

namespace NBT.Test
{
    public partial class TagCompoundTests : TestBase
    {
        #region  Tests

        [Fact]
        public void Contains_returns_false_if_not_found()
        {
            // arrange
            var target = new TagCompound();

            target.Value.Add("Beta", 10);
            target.Value.Add("Alpha", 11);
            target.Value.Add("Gamma", 12);

            // act
            var actual = target.Contains("Delta");

            // assert
            Assert.False(actual);
        }

        [Fact]
        public void Contains_returns_true_if_found()
        {
            // arrange
            var target = new TagCompound();

            target.Value.Add("Beta", 10);
            target.Value.Add("Alpha", 11);
            target.Value.Add("Gamma", 12);

            // act
            var actual = target.Contains("Alpha");

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void Count_returns_child_count()
        {
            // arrange
            const int expected = 3;
            var target = new TagCompound();

            target.Value.Add("Beta", 10);
            target.Value.Add("Alpha", 11);
            target.Value.Add("Gamma", 12);

            // act
            var actual = target.Count;

            // assert
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Count_returns_zero_for_new_compound()
        {
            // arrange
            const int expected = 0;
            var target = new TagCompound();

            // act
            var actual = target.Count;

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Indexer_returns_item_by_index()
        {
            // arrange
            var target = new TagCompound();
            Tag expected = new TagByte("Alpha", 10);

            target.Value.Add(new TagByte("Beta", 10));
            target.Value.Add(expected);
            target.Value.Add(new TagInt("Gamma"));

            // act
            var actual = target[1];

            // assert
            Assert.Same(actual, expected);
        }

        [Fact]
        public void Indexer_returns_item_by_name()
        {
            // arrange
            var target = new TagCompound();
            Tag expected = new TagByte("Alpha", 10);

            target.Value.Add(new TagByte("Beta", 10));
            target.Value.Add(expected);
            target.Value.Add(new TagInt("Gamma"));

            // act
            var actual = target["Alpha"];

            // assert
            Assert.Same(actual, expected);
        }

        [Fact]
        public void ListType_throws_exception_if_set()
        {
            // arrange
            var target = new TagCompound();

            // act
            var e = Assert.Throws<NotSupportedException>(() => ((ICollectionTag) target).ListType = TagType.Byte);
            Assert.Equal("Compounds cannot be restricted to a single type.", e.Message);
        }

        [Fact]
        public void Value_throws_exception_if_set_to_null_value()
        {
            // arrange
            var target = new TagCompound();

            // act
            var e = Assert.Throws<ArgumentNullException>(() => target.Value = null);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: value", e.Message);
        }

        #endregion
    }
}