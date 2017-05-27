using System;
using System.ComponentModel;

namespace NBT
{
    public sealed class TagString : Tag, IEquatable<TagString>
    {
        #region Constructors

        public TagString()
            : this(string.Empty, string.Empty)
        {
        }

        public TagString(string name)
            : this(name, string.Empty)
        {
        }

        public TagString(string name, string value)
            : base(name)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public override TagType Type { get; } = TagType.String;

        //TODO: Category
        //[Category("Data")]
        [DefaultValue("")]
        public string Value { get; set; }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash;

                hash = 17;
                hash = hash * 23 + Name.GetHashCode();
                hash = hash * 23 + ToString().GetHashCode();

                return hash;
            }
        }

        public override object GetValue()
        {
            return Value;
        }

        public override void SetValue(object value)
        {
            Value = Convert.ToString(value);
        }

        public override string ToString()
        {
            return string.Concat("[String: ", Name, "=\"", ToValueString(), "\"]");
        }

        public override string ToValueString()
        {
            return Value ?? string.Empty;
        }

        #endregion

        #region IEquatable<TagString> Interface

        public bool Equals(TagString other)
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