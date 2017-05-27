using System;
using System.ComponentModel;
using System.Globalization;

namespace NBT
{
    public sealed class TagByte : Tag, IEquatable<TagByte>
    {

        #region Constructors

        public TagByte()
            : this(string.Empty, 0)
        {
        }

        public TagByte(string name)
            : this(name, 0)
        {
        }

        public TagByte(byte value)
            : this(string.Empty, value)
        {
        }

        public TagByte(string name, byte value)
            : base(name)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public override TagType Type => TagType.Byte;

        //TODO: Category
        //[Category("Data")]
        [DefaultValue((byte) 0)]
        public byte Value { get; set; }

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
            Value = Convert.ToByte(value);
        }

        public override string ToValueString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region IEquatable<TagByte> Interface

        public bool Equals(TagByte other)
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