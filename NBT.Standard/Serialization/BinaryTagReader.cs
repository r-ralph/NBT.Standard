using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NBT.Serialization
{
    public class BinaryTagReader : TagReader
    {
        #region Constants

        private readonly Stream _originalStream;

        private readonly TagState _state;

        private readonly Stream _stream;

        #endregion

        #region Constructors

        public BinaryTagReader(Stream stream)
            : this(stream, true)
        { }

        public BinaryTagReader(Stream stream, bool autoDetectCompression)
        {
            if (stream.CanSeek && autoDetectCompression)
            {
                if (stream.IsGzipCompressed())
                {
                    _originalStream = stream;
                    _stream = new GZipStream(_originalStream, CompressionMode.Decompress);
                }
                else if (stream.IsDeflateCompressed())
                {
                    _originalStream = stream;
                    _stream = new DeflateStream(_originalStream, CompressionMode.Decompress);
                }
                else
                {
                    _stream = stream;
                }
            }
            else
            {
                _stream = stream;
            }

            _state = new TagState(FileAccess.Read);
            _state.Start();
        }

        #endregion

        #region Methods

        public override bool IsNbtDocument()
        {
            bool result;
            Stream stream;
            long position;

            stream = _originalStream ?? _stream;

            if (stream.CanSeek)
            {
                position = stream.Position;
            }
            else
            {
                position = -1;
            }

            if (stream.IsGzipCompressed())
            {
                using (Stream decompressionStream = new GZipStream(stream, CompressionMode.Decompress, true))
                {
                    result = decompressionStream.ReadByte() == (int)TagType.Compound;
                }
            }
            else if (stream.IsDeflateCompressed())
            {
                using (Stream decompressionStream = new DeflateStream(stream, CompressionMode.Decompress, true))
                {
                    result = decompressionStream.ReadByte() == (int)TagType.Compound;
                }
            }
            else if (stream.ReadByte() == (int)TagType.Compound)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            if (stream.CanSeek)
            {
                stream.Position = position;
            }

            return result;
        }

        public override byte ReadByte()
        {
            int data;

            data = _stream.ReadByte();

            if (data != (data & 0xFF))
            {
                throw new InvalidDataException();
            }

            return (byte)data;
        }

        public override byte[] ReadByteArray()
        {
            int length;

            length = ReadInt();
            var data = new byte[length];

            if (length != _stream.Read(data, 0, length))
            {
                throw new InvalidDataException();
            }

            return data;
        }

        public override TagDictionary ReadCompound()
        {
            var results = new TagDictionary();

            var tag = ReadTag();

            while (tag.Type != TagType.End)
            {
                results.Add(tag);
                tag = ReadTag();
            }

            return results;
        }

        public override double ReadDouble()
        {
            var data = new byte[BitHelper.DoubleSize];

            if (BitHelper.DoubleSize != _stream.Read(data, 0, BitHelper.DoubleSize))
            {
                throw new InvalidDataException();
            }

            if (TagWriter.IsLittleEndian)
            {
                BitHelper.SwapBytes(data, 0, BitHelper.DoubleSize);
            }

            return BitConverter.ToDouble(data, 0);
        }

        public override float ReadFloat()
        {
            var data = new byte[BitHelper.FloatSize];

            if (BitHelper.FloatSize != _stream.Read(data, 0, BitHelper.FloatSize))
            {
                throw new InvalidDataException();
            }

            if (TagWriter.IsLittleEndian)
            {
                BitHelper.SwapBytes(data, 0, BitHelper.FloatSize);
            }

            return BitConverter.ToSingle(data, 0);
        }

        public override int ReadInt()
        {
            var data = new byte[BitHelper.IntSize];

            if (BitHelper.IntSize != _stream.Read(data, 0, BitHelper.IntSize))
            {
                throw new InvalidDataException();
            }

            if (TagWriter.IsLittleEndian)
            {
                BitHelper.SwapBytes(data, 0, BitHelper.IntSize);
            }

            return BitConverter.ToInt32(data, 0);
        }

        public override int[] ReadIntArray()
        {
            int length;
            int bufferLength;
            bool isLittleEndian;

            isLittleEndian = TagWriter.IsLittleEndian;
            length = ReadInt();
            bufferLength = length * BitHelper.IntSize;
            var buffer = new byte[bufferLength];

            if (bufferLength != _stream.Read(buffer, 0, bufferLength))
            {
                throw new InvalidDataException();
            }

            var values = new int[length];

            for (var i = 0; i < length; i++)
            {
                if (isLittleEndian)
                {
                    BitHelper.SwapBytes(buffer, i * 4, 4);
                }

                values[i] = BitConverter.ToInt32(buffer, i * 4);
            }

            return values;
        }

        public override TagCollection ReadList()
        {
            int length;

            var listType = (TagType)ReadByte();

            if (listType < TagType.Byte || listType > TagType.IntArray)
            {
                throw new InvalidDataException($"Unexpected list type '{listType}' found.");
            }

            var tags = new TagCollection(listType);
            length = ReadInt();

            for (int i = 0; i < length; i++)
            {
                Tag tag;

                tag = null;

                _state.StartTag(listType);

                switch (listType)
                {
                    case TagType.Byte:
                        tag = TagFactory.CreateTag(ReadByte());
                        break;

                    case TagType.ByteArray:
                        tag = TagFactory.CreateTag(ReadByteArray());
                        break;

                    case TagType.Compound:
                        tag = TagFactory.CreateTag(ReadCompound());
                        break;

                    case TagType.Double:
                        tag = TagFactory.CreateTag(ReadDouble());
                        break;

                    case TagType.Float:
                        tag = TagFactory.CreateTag(ReadFloat());
                        break;

                    case TagType.Int:
                        tag = TagFactory.CreateTag(ReadInt());
                        break;

                    case TagType.IntArray:
                        tag = TagFactory.CreateTag(ReadIntArray());
                        break;

                    case TagType.List:
                        tag = TagFactory.CreateTag(ReadList());
                        break;

                    case TagType.Long:
                        tag = TagFactory.CreateTag(ReadLong());
                        break;

                    case TagType.Short:
                        tag = TagFactory.CreateTag(ReadShort());
                        break;

                    case TagType.String:
                        tag = TagFactory.CreateTag(ReadString());
                        break;

                    // Can never be hit due to the type check above
                    //default:
                    //  throw new InvalidDataException("Invalid list type.");
                }

                _state.EndTag();

                tags.Add(tag);
            }

            return tags;
        }

        public override long ReadLong()
        {
            var data = new byte[BitHelper.LongSize];

            if (BitHelper.LongSize != _stream.Read(data, 0, BitHelper.LongSize))
            {
                throw new InvalidDataException();
            }

            if (TagWriter.IsLittleEndian)
            {
                BitHelper.SwapBytes(data, 0, BitHelper.LongSize);
            }

            return BitConverter.ToInt64(data, 0);
        }

        public override short ReadShort()
        {
            var data = new byte[BitHelper.ShortSize];

            if (BitHelper.ShortSize != _stream.Read(data, 0, BitHelper.ShortSize))
            {
                throw new InvalidDataException();
            }

            if (TagWriter.IsLittleEndian)
            {
                BitHelper.SwapBytes(data, 0, BitHelper.ShortSize);
            }

            return BitConverter.ToInt16(data, 0);
        }

        public override string ReadString()
        {
            short length;

            length = ReadShort();
            var data = new byte[length];

            if (length != _stream.Read(data, 0, length))
            {
                throw new InvalidDataException();
            }

            return data.Length != 0 ? Encoding.UTF8.GetString(data) : null;
        }

        public override Tag ReadTag()
        {
            string name;

            var type = ReadTagType();

            if (type > TagType.IntArray)
            {
                throw new InvalidDataException($"Unrecognized tag type: {type}.");
            }

            var state = _state.StartTag(type);

            if (type != TagType.End && (state == null || state.ContainerType != TagType.List))
            {
                name = ReadTagName();
            }
            else
            {
                name = string.Empty;
            }

            Tag result = null;

            switch (type)
            {
                case TagType.End:
                    result = TagFactory.CreateTag(TagType.End);
                    break;

                case TagType.Byte:
                    result = TagFactory.CreateTag(name, ReadByte());
                    break;

                case TagType.Short:
                    result = TagFactory.CreateTag(name, ReadShort());
                    break;

                case TagType.Int:
                    result = TagFactory.CreateTag(name, ReadInt());
                    break;

                case TagType.IntArray:
                    result = TagFactory.CreateTag(name, ReadIntArray());
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
            }

            _state.EndTag();

            return result;
        }

        public override string ReadTagName()
        {
            return ReadString();
        }

        public override TagType ReadTagType()
        {
            int type;

            type = _stream.ReadByte();

            return (TagType)type;
        }

        #endregion
    }
}