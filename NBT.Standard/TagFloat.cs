using System;
using System.ComponentModel;
using System.Globalization;

namespace NBT
{
    public sealed class TagFloat : Tag, IEquatable<TagFloat>
    {
        #region Constructors

        public TagFloat()
            : this(string.Empty, 0)
        {
        }

        public TagFloat(string name)
            : this(name, 0)
        {
        }

        public TagFloat(float value)
            : this(string.Empty, value)
        {
        }

        public TagFloat(string name, float value)
            : base(name)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public override TagType Type => TagType.Float;

        //TODO: Category
        //[Category("Data")]
        [DefaultValue(0F)]
        public float Value { get; set; }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash;

                hash = 17;
                hash = hash * 23 + Name.GetHashCode();
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
            Value = Convert.ToSingle(value);
        }

        public override string ToValueString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region IEquatable<TagFloat> Interface

        public bool Equals(TagFloat other)
        {
            bool result;

            result = !ReferenceEquals(null, other);

            if (result && !ReferenceEquals(this, other))
            {
                result = string.Equals(Name, other.Name);

                if (result)
                {
                    result = Math.Abs(Value - other.Value) < float.Epsilon;
                }
            }

            return result;
        }

        #endregion
    }
}