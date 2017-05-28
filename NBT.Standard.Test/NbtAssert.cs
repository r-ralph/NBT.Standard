using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace NBT.Test
{
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public static class NbtAssert
    {
        #region Static Methods

        public static void Equal(TagCompound expected, TagCompound actual)
        {
            EqualBasic(expected, actual);

            ICollectionTag expectedChildren = expected;
            ICollectionTag actualChildren = actual;

            Assert.Equal(expectedChildren.IsList, actualChildren.IsList);
            Assert.Equal(expectedChildren.ListType, actualChildren.ListType);
            Assert.Equal(expectedChildren.Values.Count, actualChildren.Values.Count);

            var expectedChildValues = new List<Tag>(expectedChildren.Values);
            var actualChildValues = new List<Tag>(actualChildren.Values);

            for (var i = 0; i < expectedChildValues.Count; i++)
            {
                Equal(expectedChildValues[i], actualChildValues[i]);
            }
        }

        public static void Equal(Tag expected, Tag actual)
        {
            EqualBasic(expected, actual);

            var expectedCompound = expected as TagCompound;
            var actualCompound = actual as TagCompound;

            if (expectedCompound != null && actualCompound != null)
            {
                Equal(expectedCompound, actualCompound);
            }
        }

        public static void Equal(ComplexData expected, ComplexData actual)
        {
            Assert.Equal(expected.LongValue, actual.LongValue);
            Assert.Equal(expected.ShortValue, actual.ShortValue);
            Assert.Equal(expected.StringValue, actual.StringValue);
            Assert.Equal(expected.FloatValue, actual.FloatValue);
            Assert.Equal(expected.IntegerValue, actual.IntegerValue);
            Equal(expected.CompoundValue, actual.CompoundValue);
            Assert.Equal(expected.LongList, actual.LongList);
            Equal(expected.CompoundList, actual.CompoundList);
            Assert.Equal(expected.ByteArrayValue, actual.ByteArrayValue);
            Assert.Equal(expected.ByteValue, actual.ByteValue);
            Assert.Equal(expected.DoubleValue, actual.DoubleValue);
        }

        internal static void Equal(NbtDocument expected, NbtDocument actual)
        {
            Equal(expected.DocumentRoot, actual.DocumentRoot);
        }

        private static void Equal(List<ComplexData.Compound2> expected, List<ComplexData.Compound2> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                Equal(expected[i], actual[i]);
            }
        }

        private static void Equal(ComplexData.Compound2 expected, ComplexData.Compound2 actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.CreatedOn, actual.CreatedOn);
        }

        private static void Equal(Dictionary<string, ComplexData.Compound1> expected,
            Dictionary<string, ComplexData.Compound1> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            foreach (var pair in expected)
            {
                Equal(pair.Value, actual[pair.Key]);
            }
        }

        private static void Equal(ComplexData.Compound1 expected, ComplexData.Compound1 actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Value, actual.Value);
        }

        private static void EqualBasic(Tag expected, Tag actual)
        {
            Assert.Equal(expected.Type, actual.Type);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.FullPath, actual.FullPath);

            if (expected.Parent == null)
            {
                Assert.Null(actual.Parent);
            }
            else
            {
                Assert.Equal(expected.Parent.Name, actual.Parent.Name);
            }
        }

        #endregion
    }
}