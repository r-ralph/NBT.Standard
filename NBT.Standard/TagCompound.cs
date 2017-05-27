using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace NBT
{
    public sealed class TagCompound : Tag, ICollectionTag, IEquatable<TagCompound>
    {
        #region Constants

        private static readonly char[] QueryDelimiters =
        {
            '\\',
            '/'
        };

        #endregion

        #region Fields

        private TagDictionary _value;

        #endregion

        #region Constructors

        public TagCompound()
            : this(string.Empty)
        {
        }

        public TagCompound(string name)
            : this(name, new TagDictionary())
        {
        }

        public TagCompound(TagDictionary value)
            : this(string.Empty, value)
        {
        }

        public TagCompound(string name, TagDictionary value)
            : base(name)
        {
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of child <see cref="Tag"/> objects actually contained in the <see cref="TagCompound"/>.
        /// </summary>
        public int Count => _value.Count;

        /// <summary>
        /// Gets the <see cref="Tag"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the tag to get.</param>
        /// <returns>
        /// The <see cref="Tag"/> with the specified name. If a tag with the specified name is not found, an exception is thrown.
        /// </returns>
        public Tag this[string name] => _value[name];

        /// <summary>
        /// Gets the <see cref="Tag"/> at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the entry to access.</param>
        /// <returns>
        /// The <see cref="Tag"/> at the specified index.
        /// </returns>
        public Tag this[int index] => _value[index];

        /// <inheritdoc cref="Tag.Type"/>
        public override TagType Type => TagType.Compound;

        //TODO: Category
        //[Category("Data")]
        [DefaultValue(typeof(TagDictionary), null)]
        public TagDictionary Value
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

        public bool Contains(string name)
        {
            return Value.Contains(name);
        }

        public bool GetBooleanValue(string name)
        {
            return GetBooleanValue(name, false);
        }

        public bool GetBooleanValue(string name, bool defaultValue)
        {
            var value = GetTag<TagByte>(name);

            return value != null ? value.Value != 0 : defaultValue;
        }

        public TagByte GetByte(string name)
        {
            return GetTag<TagByte>(name);
        }

        public TagByteArray GetByteArray(string name)
        {
            return GetTag<TagByteArray>(name);
        }

        public byte[] GetByteArrayValue(string name)
        {
            return GetByteArrayValue(name, new byte[0]);
        }

        public byte[] GetByteArrayValue(string name, byte[] defaultValue)
        {
            var value = GetTag<TagByteArray>(name);

            return value != null ? value.Value : defaultValue;
        }

        public byte GetByteValue(string name)
        {
            return GetByteValue(name, default(byte));
        }

        public byte GetByteValue(string name, byte defaultValue)
        {
            var value = GetTag<TagByte>(name);

            return value?.Value ?? defaultValue;
        }

        public TagCompound GetCompound(string name)
        {
            return GetTag<TagCompound>(name);
        }

        public DateTime GetDateTimeValue(string name)
        {
            return GetDateTimeValue(name, DateTime.MinValue);
        }

        public DateTime GetDateTimeValue(string name, DateTime defaultValue)
        {
            var value = GetTag<TagString>(name);

            return value != null
                ? DateTime.Parse(value.Value, CultureInfo.InvariantCulture).ToUniversalTime()
                : defaultValue;
        }

        public TagDouble GetDouble(string name)
        {
            return GetTag<TagDouble>(name);
        }

        public double GetDoubleValue(string name)
        {
            return GetDoubleValue(name, 0);
        }

        public double GetDoubleValue(string name, double defaultValue)
        {
            var value = GetTag<TagDouble>(name);

            return value?.Value ?? defaultValue;
        }

        public TagFloat GetFloat(string name)
        {
            return GetTag<TagFloat>(name);
        }

        public float GetFloatValue(string name)
        {
            return GetFloatValue(name, 0);
        }

        public float GetFloatValue(string name, float defaultValue)
        {
            var value = GetTag<TagFloat>(name);

            return value?.Value ?? defaultValue;
        }

        public Guid GetGuidValue(string name)
        {
            return GetGuidValue(name, Guid.Empty);
        }

        public Guid GetGuidValue(string name, Guid defaultValue)
        {
            var tag = GetByteArray(name);

            return tag != null ? new Guid(tag.Value) : defaultValue;
        }

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

        public TagInt GetInt(string name)
        {
            return GetTag<TagInt>(name);
        }

        public TagIntArray GetIntArray(string name)
        {
            return GetTag<TagIntArray>(name);
        }

        public int[] GetIntArrayValue(string name)
        {
            return GetIntArrayValue(name, new int[0]);
        }

        public int[] GetIntArrayValue(string name, int[] defaultValue)
        {
            var value = GetTag<TagIntArray>(name);

            return value != null ? value.Value : defaultValue;
        }

        public int GetIntValue(string name)
        {
            return GetIntValue(name, 0);
        }

        public int GetIntValue(string name, int defaultValue)
        {
            var value = GetTag<TagInt>(name);

            return value?.Value ?? defaultValue;
        }

        public TagList GetList(string name)
        {
            return GetTag<TagList>(name);
        }

        public TagLong GetLong(string name)
        {
            return GetTag<TagLong>(name);
        }

        public long GetLongValue(string name)
        {
            return GetLongValue(name, 0);
        }

        public long GetLongValue(string name, long defaultValue)
        {
            var value = GetTag<TagLong>(name);

            return value?.Value ?? defaultValue;
        }

        public TagShort GetShort(string name)
        {
            return GetTag<TagShort>(name);
        }

        public short GetShortValue(string name)
        {
            return GetShortValue(name, 0);
        }

        public short GetShortValue(string name, short defaultValue)
        {
            var value = GetTag<TagShort>(name);

            return value?.Value ?? defaultValue;
        }

        public TagString GetString(string name)
        {
            return GetTag<TagString>(name);
        }

        public string GetStringValue(string name)
        {
            return GetStringValue(name, null);
        }

        public string GetStringValue(string name, string defaultValue)
        {
            var value = GetTag<TagString>(name);

            return value != null ? value.Value : defaultValue;
        }

        public T GetTag<T>(string name) where T : Tag
        {
            Value.TryGetValue(name, out Tag value);
            return (T) value;
        }

        public Tag GetTag(string name)
        {
            return GetTag<Tag>(name);
        }

        public override object GetValue()
        {
            return _value;
        }

        public Tag Query(string query)
        {
            return Query<Tag>(query);
        }

        public T Query<T>(string query) where T : Tag
        {
            var parts = query.Split(QueryDelimiters);
            Tag element = this;
            var failed = false;

            // HACK: This is all quickly thrown together

            foreach (var part in parts)
            {
                if (part.IndexOf('[') != -1)
                {
                    int attributePosition;
                    bool matchFound;

                    attributePosition = part.IndexOf('=');
                    matchFound = false;

                    if (attributePosition != -1)
                    {
                        var name = part.Substring(1, attributePosition - 1);
                        var value = part.Substring(attributePosition + 1, part.Length - (attributePosition + 2));
                        var list = element as TagList;

                        if (list != null)
                        {
                            // ReSharper disable once LoopCanBePartlyConvertedToQuery
                            foreach (var tag in list.Value)
                            {
                                var compound = tag as TagCompound;

                                if (compound != null && compound.GetStringValue(name) == value)
                                {
                                    element = tag;
                                    matchFound = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!matchFound)
                    {
                        // attribute not found or not set
                        failed = true;
                        break;
                    }
                }
                else
                {
                    var container = element as ICollectionTag;

                    if (container != null && container.IsList)
                    {
                        // list entry
                        int index;

                        if (int.TryParse(part, out index) && index < container.Values.Count)
                        {
                            element = container.Values[Convert.ToInt32(part)];
                        }
                        else
                        {
                            // invalid index, or out of bounds
                            failed = true;
                            break;
                        }
                    }
                    else
                    {
                        // compoound
                        var compound = (TagCompound) element;

                        if (!compound.Value.TryGetValue(part, out element))
                        {
                            // didn't find a matching key
                            failed = true;
                            break;
                        }
                    }
                }
            }

            return !failed ? (T) element : null;
        }

        public T QueryValue<T>(string query)
        {
            return QueryValue(query, default(T));
        }

        public T QueryValue<T>(string query, T defaultValue)
        {
            var tag = Query<Tag>(query);

            return tag != null ? (T) tag.GetValue() : defaultValue;
        }

        public override void SetValue(object value)
        {
            Value = (TagDictionary) value;
        }

        public override string ToString()
        {
            int count;

            count = _value?.Count ?? 0;

            return string.Concat("[", Type, ": ", Name, "] (", count.ToString(CultureInfo.InvariantCulture),
                " items)");
        }

        public override string ToValueString()
        {
            return _value?.ToString() ?? string.Empty;
        }

        #endregion

        #region ICollectionTag Interface

        bool ICollectionTag.IsList => false;

        TagType ICollectionTag.ListType
        {
            get => TagType.None;
            set => throw new NotSupportedException("Compounds cannot be restricted to a single type.");
        }

        IList<Tag> ICollectionTag.Values => Value;

        #endregion

        #region IEquatable<TagCompound> Interface

        public bool Equals(TagCompound other)
        {
            bool result;

            result = !ReferenceEquals(null, other);

            if (result && !ReferenceEquals(this, other))
            {
                result = string.Equals(Name, other.Name);

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