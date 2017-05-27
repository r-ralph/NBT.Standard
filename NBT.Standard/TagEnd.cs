using System;

namespace NBT
{
    public sealed class TagEnd : Tag, IEquatable<TagEnd>
    {
        #region Constants

        private const string Value = "[End]";

        #endregion

        #region Constructors

        public TagEnd()
            : base(string.Empty)
        {
        }

        #endregion

        #region Properties

        public override TagType Type => TagType.End;

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override object GetValue()
        {
            throw new NotSupportedException("Tag does not support values.");
        }

        public override void SetValue(object value)
        {
            throw new NotSupportedException("Tag does not support values.");
        }

        public override string ToString()
        {
            return Value;
        }

        public override string ToValueString()
        {
            return string.Empty;
        }

        #endregion

        #region IEquatable<TagEnd> Interface

        public bool Equals(TagEnd other)
        {
            return !ReferenceEquals(null, other);
        }

        #endregion
    }
}