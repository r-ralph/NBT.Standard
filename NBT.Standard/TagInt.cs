using System;
using System.ComponentModel;
using System.Globalization;

namespace NBT
{
    public sealed class TagInt : Tag, IEquatable<TagInt>
    {
        #region Constructors

        public TagInt()
            : this(string.Empty, 0)
        {
        }

        public TagInt(string name)
            : this(name, 0)
        {
        }

        public TagInt(int value)
            : this(string.Empty, value)
        {
        }

        public TagInt(string name, int value)
            : base(name)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public override TagType Type => TagType.Int;

        //TODO: Category
        //[Category("Data")]
        [DefaultValue(0)]
        public int Value { get; set; }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash;

                hash = 17;
                hash = hash * 23 + Name.GetHashCode();
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                hash = hash * 23 + Value.GetHashCode();

                return hash;
            }
        }

        public override object GetValue()
        {
            return Value;
        }

        public override void SetValue(object value)
        {
            Value = Convert.ToInt32(value);
        }

        public override string ToValueString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region IEquatable<TagInt> Interface

        public bool Equals(TagInt other)
        {
            bool result;

            result = !ReferenceEquals(null, other);

            if (result && !ReferenceEquals(this, other))
            {
                result = string.Equals(Name, other.Name);

                if (result)
                {
                    result = Value == other.Value;
                }
            }

            return result;
        }

        #endregion
    }
}