using System;
using Xunit;

namespace NBT.Test
{
    public class TagEndTests : TestBase
    {
        #region  Tests

        [Fact]
        public void ConstructorTest()
        {
            // arrange
            // act
            var target = new TagEnd();

            // assert
            Assert.Empty(target.Name);
        }

        [Fact]
        public void Equals_returns_true_for_any_end_tag()
        {
            // arrange
            var target = new TagEnd();
            var other = new TagEnd();

            // act
            var actual = target.Equals(other);

            // assert
            Assert.True(actual);
        }

        [Fact]
        public void GetHashCode_returns_same_value()
        {
            // arrange
            var target = new TagEnd();
            var expected = new TagEnd().GetHashCode();

            // act
            var actual = target.GetHashCode();

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValue_throws_exception()
        {
            // arrange
            var target = new TagEnd();

            // act
            var e = Assert.Throws<NotSupportedException>(() => target.GetValue());
            Assert.Equal("Tag does not support values.", e.Message);
        }

        [Fact]
        public void SetValue_throws_exception()
        {
            // arrange
            var target = new TagEnd();

            // act
            var e = Assert.Throws<NotSupportedException>(() => target.SetValue("TEST"));
            Assert.Equal("Tag does not support values.", e.Message);
        }

        [Fact]
        public void ToStringTest()
        {
            // arrange
            const string expected = "[End]";
            var target = new TagEnd();

            // act
            var actual = target.ToString();

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToValueString_returns_empty_value()
        {
            // arrange
            var target = new TagEnd();

            // act
            var actual = target.ToValueString();

            // assert
            Assert.Empty(actual);
        }

        [Fact]
        public void TypeTest()
        {
            // arrange
            const TagType expected = TagType.End;

            // act
            var actual = new TagEnd().Type;

            // assert
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}