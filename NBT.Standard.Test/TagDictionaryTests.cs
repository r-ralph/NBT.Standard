using System;
using System.Collections.Generic;
using Xunit;

namespace NBT.Test
{
    partial class TagDictionaryTests
    {
        #region  Tests

        [Fact]
        public void Add_sets_parent()
        {
            // arrange
            var actual = new TagByte("alpha", 56);
            var owner = new TagCompound();
            var target = owner.Value;

            // act
            target.Add(actual);

            // assert
            Assert.Same(owner, actual.Parent);
        }

        [Fact]
        public void Add_throws_exception_for_unsupported_data_type()
        {
            // arrange
            var target = new TagDictionary();

            // act
            var e = Assert.Throws<ArgumentException>(() => target.Add("alpha", TimeSpan.MinValue));
            Assert.Equal("Invalid value type.\r\nParameter name: value", e.Message);
        }

        [Fact]
        public void AddRange_adds_dictionary_contents()
        {
            // arrange
            var target = new TagDictionary();
            var expected = new Dictionary<string, object>
            {
                {
                    "alpha", (byte) 1
                },
                {
                    "beta", short.MaxValue
                },
                {
                    "gamma", int.MaxValue
                }
            };

            // act
            target.AddRange(expected);

            // assert
            Assert.Equal(expected.Count, target.Count);
            Assert.IsType<TagByte>(target["alpha"]);
            Assert.Equal(1, target["alpha"].GetValue());
            Assert.IsType<TagShort>(target["beta"]);
            Assert.Equal(short.MaxValue, target["beta"].GetValue());
            Assert.IsType<TagInt>(target["gamma"]);
            Assert.Equal(int.MaxValue, target["gamma"].GetValue());
        }

        [Fact]
        public void AddRange_adds_multiple__key_value_pairs()
        {
            // arrange
            var target = new TagDictionary();
            var expected = new[]
            {
                new KeyValuePair<string, object>("alpha", (byte) 1),
                new KeyValuePair<string, object>("beta", short.MaxValue),
                new KeyValuePair<string, object>("gamma", int.MaxValue)
            };

            // act
            target.AddRange(expected);

            // assert
            Assert.Equal(expected.Length, target.Count);
            Assert.IsType<TagByte>(target["alpha"]);
            Assert.Equal(1, target["alpha"].GetValue());
            Assert.IsType<TagShort>(target["beta"]);
            Assert.Equal(short.MaxValue, target["beta"].GetValue());
            Assert.IsType<TagInt>(target["gamma"]);
            Assert.Equal(int.MaxValue, target["gamma"].GetValue());
        }

        [Fact]
        public void AddRange_adds_tags()
        {
            // arrange
            var target = new TagDictionary();
            var expected = new Tag[]
            {
                new TagByte("alpha", 1),
                new TagShort("beta", short.MaxValue),
                new TagInt("gamma", int.MaxValue)
            };

            // act
            target.AddRange(expected);

            // assert
            Assert.Equal(expected.Length, target.Count);
            Assert.Same(target["alpha"], expected[0]);
            Assert.Same(target["beta"], expected[1]);
            Assert.Same(target["gamma"], expected[2]);
        }

        [Fact]
        public void Changing_tag_name_updates_key()
        {
            // arrange
            var actual = new TagByte("alpha", 56);
            var owner = new TagCompound();
            var target = owner.Value;

            target.Add(actual);

            // act
            actual.Name = "beta";

            // assert
            Assert.False(target.Contains("alpha"));
            Assert.True(target.Contains("beta"));
            Assert.Same(actual, target["beta"]);
        }

        [Fact]
        public void Clear_removes_parents()
        {
            // arrange
            var actual1 = new TagByte("alpha", 56);
            var actual2 = new TagShort("beta", 56);

            var owner = new TagCompound();
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
        public void Remove_clears_parent()
        {
            // arrange
            var owner = new TagCompound();
            var target = owner.Value;

            var actual = target.Add("alpha", (byte) 56);

            // act
            target.Remove(actual);

            // assert
            Assert.Null(actual.Parent);
        }

        #endregion
    }
}