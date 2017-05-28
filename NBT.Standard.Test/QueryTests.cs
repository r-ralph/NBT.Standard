using Xunit;

namespace NBT.Test
{
    public class QueryTests : TestBase
    {
        #region  Tests

        [Fact]
        public void Query_handles_attribute_mismatches()
        {
            // arrange
            var target = CreateComplexData();
            const string path = @"listTest (compound)/[name=NOMANS]";

            // act
            var actual = target.Query<TagCompound>(path);

            // assert
            Assert.Null(actual);
        }

        [Fact]
        public void Query_handles_invalid_compound_name()
        {
            // arrange
            var target = CreateComplexData();
            const string path = @"nested compound test\Dilbert";

            // act
            var actual = target.Query(path);

            // assert
            Assert.Null(actual);
        }

        [Fact]
        public void Query_handles_invalid_list_index()
        {
            // arrange
            var target = CreateComplexData();
            const string path = @"listTest (compound)\100\created-on";

            // act
            var actual = target.Query(path);

            // assert
            Assert.Null(actual);
        }

        [Fact]
        public void QueryValueTest()
        {
            // arrange
            var target = CreateComplexData();
            const string path = @"listTest (compound)\1\created-on";
            const long expectedValue = 1264099775885;

            // act
            var result = target.QueryValue<long>(path);

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void QueryWithAttributeTest()
        {
            // arrange
            var target = CreateComplexData();
            const string path = @"listTest (compound)/[name=Compound tag #0]";
            const string expectedValue = "Compound tag #0";

            // act
            var result = target.Query<TagCompound>(path);

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result.Value["name"].GetValue());
        }

        [Fact]
        public void QueryWithExplicitTypeTest()
        {
            // arrange
            var target = CreateComplexData();
            const string path = @"listTest (compound)\0\name";
            const string expectedValue = "Compound tag #0";

            // act
            var result = target.Query<TagString>(path);

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result.Value);
        }

        [Fact]
        public void QueryWithoutExplicitTypeTest()
        {
            // arrange
            var target = CreateComplexData();
            const string path = @"listTest (compound)\1\created-on";
            var expectedType = typeof(TagLong);
            const long expectedValue = 1264099775885;

            // act
            var result = target.Query(path);

            // assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom(expectedType, result);
            Assert.Equal(expectedValue, (long) result.GetValue());
        }

        #endregion
    }
}