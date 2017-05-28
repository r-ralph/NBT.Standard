using Xunit;

// sanity checks just in case the t4 generator data is screwed up

namespace NBT.Test
{
    public class TagTypeTests
    {
        #region  Tests

        [Fact]
        public void ByteArrayTest()
        {
            TestValue(7, TagType.ByteArray);
        }

        [Fact]
        public void ByteTest()
        {
            TestValue(1, TagType.Byte);
        }

        [Fact]
        public void CompoundTest()
        {
            TestValue(10, TagType.Compound);
        }

        [Fact]
        public void DoubleTest()
        {
            TestValue(6, TagType.Double);
        }

        [Fact]
        public void EndTest()
        {
            TestValue(0, TagType.End);
        }

        [Fact]
        public void FloatTest()
        {
            TestValue(5, TagType.Float);
        }

        [Fact]
        public void IntArrayTest()
        {
            TestValue(11, TagType.IntArray);
        }

        [Fact]
        public void IntTest()
        {
            TestValue(3, TagType.Int);
        }

        [Fact]
        public void ListTest()
        {
            TestValue(9, TagType.List);
        }

        [Fact]
        public void LongTest()
        {
            TestValue(4, TagType.Long);
        }

        [Fact]
        public void ShortTest()
        {
            TestValue(2, TagType.Short);
        }

        [Fact]
        public void StringTest()
        {
            TestValue(8, TagType.String);
        }

        #endregion

        #region Test Helpers

        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void TestValue(int expected, TagType value)
        {
            // act
            var actual = (int) value;

            // assert
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}