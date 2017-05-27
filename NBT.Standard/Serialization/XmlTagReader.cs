using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace NBT.Serialization
{
    public partial class XmlTagReader : TagReader
    {
        #region Constants

        private static readonly char[] ArraySeparaters =
        {
            ' ',
            '\t',
            '\n',
            '\r'
        };

        private readonly XmlReader _reader;

        private readonly TagState _state;

        #endregion

        #region Constructors

        public XmlTagReader(XmlReader reader)
        {
            _reader = reader;

            _state = new TagState(FileAccess.Read);
            _state.Start();
        }

        public XmlTagReader(Stream stream) : this(XmlReader.Create(stream))
        {
        }

        #endregion

        #region Methods

        public override void Dispose()
        {
            base.Dispose();
            _reader.Dispose();
        }

        public override bool IsNbtDocument()
        {
            bool result;
            try
            {
                InitializeReader();

                var typeName = _reader.GetAttribute("type");

                result = !string.IsNullOrEmpty(typeName) &&
                         (TagType) Enum.Parse(typeof(TagType), typeName, true) == TagType.Compound;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public override byte ReadByte()
        {
            return (byte) _reader.ReadElementContentAsInt();
        }

        public override byte[] ReadByteArray()
        {
            byte[] result;

            var value = ReadString();

            if (!string.IsNullOrEmpty(value))
            {
                var values = value.Split(ArraySeparaters, StringSplitOptions.RemoveEmptyEntries);
                result = new byte[values.Length];

                for (var i = 0; i < values.Length; i++)
                {
                    result[i] = Convert.ToByte(values[i]);
                }
            }
            else
            {
                result = TagByteArray.EmptyValue;
            }

            return result;
        }

        public override TagDictionary ReadCompound()
        {
            var value = new TagDictionary();

            _reader.Read();

            ReadChildValues(value, TagType.None);

            return value;
        }

        public override double ReadDouble()
        {
            return _reader.ReadElementContentAsDouble();
        }

        public override float ReadFloat()
        {
            return _reader.ReadElementContentAsFloat();
        }

        public override int ReadInt()
        {
            return _reader.ReadElementContentAsInt();
        }

        public override int[] ReadIntArray()
        {
            int[] result;

            var value = ReadString();

            if (!string.IsNullOrEmpty(value))
            {
                string[] values;

                values = value.Split(ArraySeparaters, StringSplitOptions.RemoveEmptyEntries);
                result = new int[values.Length];

                for (var i = 0; i < values.Length; i++)
                {
                    result[i] = Convert.ToInt32(values[i]);
                }
            }
            else
            {
                result = TagIntArray.EmptyValue;
            }

            return result;
        }

        public override TagCollection ReadList()
        {
            var listTypeName = _reader.GetAttribute("limitType");
            if (string.IsNullOrEmpty(listTypeName))
            {
                throw new InvalidDataException("Missing limitType attribute, unable to determine list contents type.");
            }

            var listType = (TagType) Enum.Parse(typeof(TagType), listTypeName, true);
            var value = new TagCollection(listType);

            _reader.Read();

            ReadChildValues(value, listType);

            return value;
        }

        public override long ReadLong()
        {
            return _reader.ReadElementContentAsLong();
        }

        public override short ReadShort()
        {
            return (short) _reader.ReadElementContentAsInt();
        }

        public override string ReadString()
        {
            var value = _reader.ReadElementContentAsString();
            if (string.IsNullOrEmpty(value))
            {
                value = null;
            }

            return value;
        }

        public override Tag ReadTag()
        {
            return ReadTag(TagType.None);
        }

        public override string ReadTagName()
        {
            return _reader.Name;
        }

        public override TagType ReadTagType()
        {
            return ReadTagType(TagType.None);
        }

        private void InitializeReader()
        {
            if (_reader.ReadState == ReadState.Initial)
            {
                _reader.MoveToContent();
            }
        }

        private void ReadChildValues(ICollection<Tag> value, TagType listType)
        {
            int depth;

            _state.StartList(listType, 0);

            SkipWhitespace();

            depth = _reader.Depth;

            if (_reader.NodeType != XmlNodeType.EndElement)
            {
                do
                {
                    if (_reader.NodeType == XmlNodeType.Element)
                    {
                        var child = ReadTag(listType);
                        if (listType != TagType.None)
                        {
                            // sanity check as depending how you
                            // decided to load documents it is
                            // currently possible to skip some checks
                            child.Name = string.Empty;
                        }

                        value.Add(child);
                    }
                    else
                    {
                        _reader.Read();
                    }
                } while (_reader.Depth == depth && _reader.ReadState == ReadState.Interactive);
            }
            else
            {
                _reader.Read();
                SkipWhitespace();
            }
        }

        private Tag ReadTag(TagType defaultTagType)
        {
            string name;

            var type = ReadTagType(defaultTagType);

            var state = _state.StartTag(type);

            if (type != TagType.End && (state == null || state.ContainerType != TagType.List))
            {
                name = _reader.GetAttribute("name");
                if (string.IsNullOrEmpty(name))
                {
                    name = ReadTagName();
                }
            }
            else
            {
                name = string.Empty;
            }

            Tag result = null;

            switch (type)
            {
                case TagType.Byte:
                    result = TagFactory.CreateTag(name, ReadByte());
                    break;

                case TagType.Short:
                    result = TagFactory.CreateTag(name, ReadShort());
                    break;

                case TagType.Int:
                    result = TagFactory.CreateTag(name, ReadInt());
                    break;

                case TagType.Long:
                    result = TagFactory.CreateTag(name, ReadLong());
                    break;

                case TagType.Float:
                    result = TagFactory.CreateTag(name, ReadFloat());
                    break;

                case TagType.Double:
                    result = TagFactory.CreateTag(name, ReadDouble());
                    break;

                case TagType.ByteArray:
                    result = TagFactory.CreateTag(name, ReadByteArray());
                    break;

                case TagType.String:
                    result = TagFactory.CreateTag(name, ReadString());
                    break;

                case TagType.List:
                    result = TagFactory.CreateTag(name, ReadList());
                    break;

                case TagType.Compound:
                    result = TagFactory.CreateTag(name, ReadCompound());
                    break;

                case TagType.IntArray:
                    result = TagFactory.CreateTag(name, ReadIntArray());
                    break;

                // Can't be hit as ReadTagType will throw
                // an exception for unsupported types
                // default:
                //   throw new InvalidDataException($"Unrecognized tag type: {type}");
            }

            _state.EndTag();

            return result;
        }

        private TagType ReadTagType(TagType defaultTagType)
        {
            TagType type;

            InitializeReader();

            if (defaultTagType != TagType.None)
            {
                type = defaultTagType;
            }
            else
            {
                var typeName = _reader.GetAttribute("type");

                if (string.IsNullOrEmpty(typeName))
                {
                    throw new InvalidDataException("Missing type attribute, unable to determine tag type.");
                }

                if (!_tagTypeEnumLookup.TryGetValue(typeName, out type))
                {
                    throw new InvalidDataException($"Unrecognized or unsupported tag type '{typeName}'.");
                }
            }

            return type;
        }

        private void SkipWhitespace()
        {
            while (_reader.NodeType == XmlNodeType.Whitespace)
            {
                _reader.Read();
            }
        }

        #endregion
    }
}