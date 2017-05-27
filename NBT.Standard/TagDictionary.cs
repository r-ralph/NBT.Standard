using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NBT
{
    public partial class TagDictionary : KeyedCollection<string, Tag>
    {
        #region Fields

        private Tag _owner;

        #endregion

        #region Properties

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

        public TagByte Add(string name, bool value)
        {
            return Add(name, (byte) (value ? 1 : 0));
        }

        public TagString Add(string name, DateTime value)
        {
            return Add(name, value.ToString("u"));
        }

        public TagByteArray Add(string name, Guid value)
        {
            return Add(name, value.ToByteArray());
        }

        public Tag Add(string name, TagType tagType)
        {
            return Add(name, tagType, TagType.None);
        }

        public Tag Add(string name, TagType tagType, TagType limitToType)
        {
            var tag = TagFactory.CreateTag(name, tagType, limitToType);

            Add(tag);

            return tag;
        }

        public Tag Add(string name, object value)
        {
            Tag result;

            if (value is byte)
            {
                result = Add(name, (byte) value);
            }
            else if (value is byte[])
            {
                result = Add(name, (byte[]) value);
            }
            else if (value is int)
            {
                result = Add(name, (int) value);
            }
            else if (value is int[])
            {
                result = Add(name, (int[]) value);
            }
            else if (value is float)
            {
                result = Add(name, (float) value);
            }
            else if (value is double)
            {
                result = Add(name, (double) value);
            }
            else if (value is long)
            {
                result = Add(name, (long) value);
            }
            else if (value is short)
            {
                result = Add(name, (short) value);
            }
            else if (value is string)
            {
                result = Add(name, (string) value);
            }
            else if (value is DateTime)
            {
                result = Add(name, (DateTime) value);
            }
            else if (value is Guid)
            {
                result = Add(name, (Guid) value);
            }
            else if (value is bool)
            {
                result = Add(name, (bool) value);
            }
            else if (value is TagDictionary)
            {
                result = Add(name, (TagDictionary) value);
            }
            else if (value is TagCollection)
            {
                result = Add(name, (TagCollection) value);
            }
            else
            {
                throw new ArgumentException("Invalid value type.", nameof(value));
            }

            return result;
        }

        /// <summary>
        /// Adds a range of existing <see cref="T:KeyValuePair{string,object}"/> objects to the <see cref="TagDictionary"/>.
        /// </summary>
        /// <param name="values">An IEnumerable&lt;Tag&gt; of items to append to the <see cref="TagDictionary"/>.</param>
        public void AddRange(IEnumerable<KeyValuePair<string, object>> values)
        {
            foreach (var value in values)
            {
                Add(value.Key, value.Value);
            }
        }

        /// <summary>
        /// Adds the contents of an existing <see cref="T:IDictionary{string,object}"/> objects to the <see cref="TagDictionary"/>.
        /// </summary>
        /// <param name="values">An IEnumerable&lt;Tag&gt; of items to append to the <see cref="TagDictionary"/>.</param>
        public void AddRange(IDictionary<string, object> values)
        {
            foreach (var value in values)
            {
                Add(value.Key, value.Value);
            }
        }

        /// <summary>
        /// Adds a range of existing <see cref="Tag"/> objects to the <see cref="TagDictionary"/>.
        /// </summary>
        /// <param name="values">An IEnumerable&lt;Tag&gt; of items to append to the <see cref="TagDictionary"/>.</param>
        public void AddRange(IEnumerable<Tag> values)
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

        public bool TryGetValue(string key, out Tag value)
        {
            bool result;

            if (Dictionary != null)
            {
                result = Dictionary.TryGetValue(key, out value);
            }
            else
            {
                result = false;
                value = null;
            }

            return result;
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                item.Parent = null;
            }

            base.ClearItems();
        }

        protected override string GetKeyForItem(Tag item)
        {
            return item.Name;
        }

        protected override void InsertItem(int index, Tag item)
        {
            item.Parent = Owner;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            item.Parent = null;

            base.RemoveItem(index);
        }

        internal void ChangeKey(Tag item, string newKey)
        {
            ChangeItemKey(item, newKey);
        }

        #endregion
    }
}