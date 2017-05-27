using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NBT
{
    public partial class TagCollection : Collection<Tag>
    {
        #region Fields

        private Tag _owner;

        #endregion

        #region Constructors

        public TagCollection()
        {
            LimitType = TagType.None;
        }

        public TagCollection(TagType limitType)
        {
            LimitType = limitType;
        }

        #endregion

        #region Properties

        public TagType LimitType { get; private set; }

        public Tag Owner
        {
            get => _owner;
            set
            {
                _owner = value;

                foreach (var child in this)
                {
                    child.Parent = value;
                }
            }
        }

        #endregion

        #region Methods

        public Tag Add(TagType tagType)
        {
            return Add(tagType, TagType.None);
        }

        public Tag Add(TagType tagType, TagType limitToType)
        {
            var tag = TagFactory.CreateTag(string.Empty, tagType, limitToType);

            Add(tag);

            return tag;
        }

        public new void Add(Tag value)
        {
            base.Add(value);
        }

        public Tag Add(object value)
        {
            Tag result;

            // ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
            if (value is byte)
            {
                result = Add((byte) value);
            }
            else if (value is byte[])
            {
                result = Add((byte[]) value);
            }
            else if (value is int)
            {
                result = Add((int) value);
            }
            else if (value is int[])
            {
                result = Add((int[]) value);
            }
            else if (value is float)
            {
                result = Add((float) value);
            }
            else if (value is double)
            {
                result = Add((double) value);
            }
            else if (value is long)
            {
                result = Add((long) value);
            }
            else if (value is short)
            {
                result = Add((short) value);
            }
            else if (value is string)
            {
                result = Add((string) value);
            }
            else if (value is TagDictionary)
            {
                result = Add((TagDictionary) value);
            }
            else if (value is TagCollection)
            {
                result = Add((TagCollection) value);
            }
            else
            {
                throw new ArgumentException("Invalid value type.", nameof(value));
            }
            // ReSharper restore CanBeReplacedWithTryCastAndCheckForNull

            return result;
        }

        public void AddRange(IEnumerable<object> values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append('[');

            foreach (var tag in this)
            {
                if (sb.Length > 1)
                {
                    sb.Append(',').Append(' ');
                }

                sb.Append(tag.ToValueString());
            }

            sb.Append(']');

            return sb.ToString();
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                item.Parent = null;
            }

            base.ClearItems();
        }

        protected override void InsertItem(int index, Tag item)
        {
            if (LimitType == TagType.None)
            {
                LimitType = item.Type;
            }
            else if (item.Type != LimitType)
            {
                throw new ArgumentException($"Only items of type {LimitType} can be added to this collection.",
                    nameof(item));
            }

            if (!string.IsNullOrEmpty(item.Name))
            {
                throw new ArgumentException("Only unnamed tags are supported.", nameof(item));
            }

            item.Parent = Owner;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            item.Parent = null;

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, Tag item)
        {
            if (LimitType != TagType.None && item.Type != LimitType)
            {
                throw new ArgumentException($"Only items of type {LimitType} can be added to this collection.",
                    nameof(item));
            }

            item.Parent = Owner;

            base.SetItem(index, item);
        }

        #endregion
    }
}