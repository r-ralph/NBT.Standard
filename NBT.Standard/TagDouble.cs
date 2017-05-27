using System;
using System.ComponentModel;
using System.Globalization;

namespace NBT
{
    public sealed class TagDouble : Tag, IEquatable<TagDouble>
    {
        #region Constructors

        public TagDouble()
            : this(string.Empty, 0)
        {
        }

        public TagDouble(string name)
            : this(name, 0)
        {
        }

        public TagDouble(double value)
            : this(string.Empty, value)
        {
        }

        public TagDouble(string name, double value)
            : base(name)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public override TagType Type => TagType.Double;

        //TODO: Category
        //[Category("Data")]
        [DefaultValue(0D)]
        public double Value { get; set; }

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
            Value = Convert.ToDouble(value);
        }

        public override string ToValueString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region IEquatable<TagDouble> Interface

        public bool Equals(TagDouble other)
        {
            bool result;

            result = !ReferenceEquals(null, other);

            if (result && !ReferenceEquals(this, other))
            {
                result = string.Equals(Name, other.Name);

                if (result)
                {
                    result = Math.Abs(Value - other.Value) < double.Epsilon;
                }
            }

            return result;
        }

        #endregion
    }
}