using System;
using System.ComponentModel;
using System.Globalization;

namespace NBT
{
    public sealed class TagShort : Tag, IEquatable<TagShort>
    {
        #region Constructors

        public TagShort()
            : this(string.Empty, 0)
        {
        }

        public TagShort(string name)
            : this(name, 0)
        {
        }

        public TagShort(short value)
            : this(string.Empty, value)
        {
        }

        public TagShort(string name, short value)
            : base(name)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public override TagType Type { get; } = TagType.Short;

        //TODO: Category
        //[Category("Data")]
        [DefaultValue((short) 0)]
        public short Value { get; set; }

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
            Value = Convert.ToInt16(value);
        }

        public override string ToValueString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region IEquatable<TagShort> Interface

        public bool Equals(TagShort other)
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