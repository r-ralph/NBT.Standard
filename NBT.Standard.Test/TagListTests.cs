using System;
using Xunit;

namespace NBT.Test
{
    public partial class TagListTests : TestBase
    {
        #region  Tests

        [Fact]
        public void Count_returns_number_of_children()
        {
            // arrange
            const int expected = 3;
            var target = new TagList(TagType.Int);
            target.Value.Add(256);
            target.Value.Add(512);
            target.Value.Add(1024);

            // act
            var actual = target.Count;

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Value_throws_exception_if_set_to_null_value()
        {
            // arrange
            var target = new TagList();

            // act
            Assert.Throws<ArgumentNullException>(() => target.Value = null);
        }

        #endregion
    }
}