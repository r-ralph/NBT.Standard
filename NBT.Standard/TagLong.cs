using System;
using System.ComponentModel;
using System.Globalization;

namespace NBT
{
    public sealed class TagLong : Tag, IEquatable<TagLong>
    {
        #region Constructors

        public TagLong()
            : this(string.Empty, 0)
        {
        }

        public TagLong(string name)
            : this(name, 0)
        {
        }

        public TagLong(long value)
            : this(string.Empty, value)
        {
        }

        public TagLong(string name, long value)
            : base(name)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public override TagType Type => TagType.Long;

        //TODO: Category
        //[Category("Data")]
        [DefaultValue(0L)]
        public long Value { get; set; }

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
            Value = Convert.ToInt64(value);
        }

        public override string ToValueString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region IEquatable<TagLong> Interface

        public bool Equals(TagLong other)
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