using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace NBT
{
    public sealed class TagList : Tag, ICollectionTag, IEquatable<TagList>
    {
        #region Fields

        private TagCollection _value;

        #endregion

        #region Constructors

        public TagList()
            : this(string.Empty)
        {
        }

        public TagList(string name)
            : this(name, TagType.None)
        {
        }

        public TagList(TagType listType)
            : this(string.Empty, listType)
        {
        }

        public TagList(TagCollection value)
            : this(string.Empty, value)
        {
        }

        public TagList(string name, TagType listType)
            : this(name, listType, new TagCollection(listType))
        {
        }

        public TagList(string name, TagCollection value)
            : this(name, value.LimitType, value)
        {
        }

        public TagList(string name, TagType listType, TagCollection value)
            : base(name)
        {
            ListType = listType;
            Value = value;
        }

        #endregion

        #region Properties

        public int Count => _value.Count;

        public override TagType Type => TagType.List;

        //TODO: Category
        //[Category("Data")]
        [DefaultValue(typeof(TagCollection), null)]
        public TagCollection Value
        {
            get => _value;
            set
            {
                if (!ReferenceEquals(_value, value))
                {
                    _value = value ?? throw new ArgumentNullException(nameof(value));
                    value.Owner = this;
                }
            }
        }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            // http://stackoverflow.com/a/263416/148962

            unchecked // Overflow is fine, just wrap
            {
                int hash;

                hash = 17;
                hash = hash * 23 + Name.GetHashCode();

                var values = Value;

                if (values != null)
                {
                    for (var i = 0; i < values.Count; i++)
                    {
                        // ReSharper disable once NonReadonlyMemberInGetHashCode
                        hash = hash * 23 + _value[i].GetHashCode();
                    }
                }

                return hash;
            }
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            Value = (TagCollection) value;
        }

        public override string ToString()
        {
            int count;

            count = _value.Count;

            return string.Concat("[", Type, ": ", Name, "] (", count.ToString(CultureInfo.InvariantCulture),
                " items)");
        }

        public override string ToValueString()
        {
            return _value.ToString() ?? string.Empty;
        }

        #endregion

        #region ICollectionTag Interface

        public TagType ListType
        {
            get => Value?.LimitType ?? TagType.None;
            set
            {
                if (Value == null || _value.LimitType != value)
                {
                    Value = new TagCollection(value);
                }
            }
        }

        bool ICollectionTag.IsList => true;

        IList<Tag> ICollectionTag.Values => Value;

        #endregion

        #region IEquatable<TagList> Interface

        public bool Equals(TagList other)
        {
            bool result;

            result = !ReferenceEquals(null, other);

            if (result && !ReferenceEquals(this, other))
            {
                result = string.Equals(Name, other.Name) && ListType == other.ListType;

                if (result)
                {
                    IList<Tag> src = Value;
                    IList<Tag> dst = other.Value;

                    result = src.Count == dst.Count;

                    for (var i = 0; i < src.Count; i++)
                    {
                        var srcTag = src[i];
                        var dstTag = dst[i];

                        if (!srcTag.Equals(dstTag))
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        #endregion
    }
}