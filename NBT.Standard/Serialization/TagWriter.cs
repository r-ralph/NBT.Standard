using System;
using System.IO;

namespace NBT.Serialization
{
    public abstract partial class TagWriter
    {
        #region Constants

        internal static readonly bool IsLittleEndian;

        #endregion

        #region Static Constructors

        static TagWriter()
        {
            IsLittleEndian = BitConverter.IsLittleEndian;
        }

        #endregion

        #region Static Methods

        public static TagWriter CreateWriter(NbtFormat format, Stream stream)
        {
            TagWriter writer;

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            switch (format)
            {
                case NbtFormat.Binary:
                    writer = new BinaryTagWriter(stream);
                    break;
                case NbtFormat.Xml:
                    writer = new XmlTagWriter(stream);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, "Invalid format.");
            }

            return writer;
        }

        #endregion

        #region Methods

        public virtual void Dispose()
        {
        }

        public abstract void Flush();

        public abstract void WriteEndDocument();

        public abstract void WriteEndTag();

        public void WriteStartArray(TagType type, int count)
        {
            WriteStartArray(string.Empty, type, count);
        }

        public abstract void WriteStartArray(string name, TagType type, int count);

        public abstract void WriteStartDocument();

        public void WriteStartTag(Tag tag)
        {
            WriteStartTag(tag.Name, tag.Type);
        }

        public void WriteStartTag(TagType type)
        {
            WriteStartTag(string.Empty, type);
        }

        public abstract void WriteStartTag(string name, TagType type);

        public abstract void WriteStartTag(string name, TagType type, TagType listType, int count);

        public void WriteTag(Tag tag)
        {
            WriteStartTag(tag);
            WriteValue(tag);
            WriteEndTag();
        }

        #endregion
    }
}