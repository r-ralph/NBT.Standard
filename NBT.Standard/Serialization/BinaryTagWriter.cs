using System;
using System.IO;
using System.Text;

namespace NBT.Serialization
{
    public class BinaryTagWriter : TagWriter
    {
        #region Constants

        private readonly TagState _state;

        private readonly Stream _stream;

        #endregion

        #region Constructors

        public BinaryTagWriter(Stream stream)
        {
            _state = new TagState(FileAccess.Write);
            _stream = stream;
        }

        #endregion

        #region Methods

        public override void Dispose()
        {
            _stream.Flush();
            _stream.Dispose();
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override void WriteArrayValue(byte value)
        {
            WriteValue(value);
        }

        public override void WriteArrayValue(int value)
        {
            WriteValue(value);
        }

        public override void WriteEndDocument()
        {
            _state.SetComplete();
        }

        public override void WriteEndTag()
        {
            _state.EndTag(WriteEnd);
        }

        public override void WriteStartArray(string name, TagType type, int count)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (type == TagType.Byte)
            {
                type = TagType.ByteArray;
            }
            else if (type == TagType.Int)
            {
                type = TagType.IntArray;
            }
            else if (type != TagType.ByteArray && type != TagType.IntArray)
            {
                throw new ArgumentException("Only byte or integer types are supported.", nameof(type));
            }

            WriteStartTag(name, type);
            WriteValue(count);
        }

        public override void WriteStartDocument()
        {
            _state.Start();
        }

        public override void WriteStartTag(string name, TagType type)
        {
            var currentState = _state.StartTag(type);

            if (type != TagType.End && (currentState == null || currentState.ContainerType != TagType.List))
            {
                WriteValue((byte) type);
                WriteValue(name);
            }
        }

        public override void WriteStartTag(string name, TagType type, TagType listType, int count)
        {
            // HACK: This is messy, rethink

            WriteStartTag(name, type);

            _state.StartList(listType, count);

            _stream.WriteByte((byte) listType);
            WriteValue(count);
        }

        protected override void WriteValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteValue((short) 0);
            }
            else
            {
                var buffer = Encoding.UTF8.GetBytes(value);

                if (buffer.Length > short.MaxValue)
                {
                    throw new ArgumentException("String data would be truncated.");
                }

                WriteValue((short) buffer.Length);
                _stream.Write(buffer, 0, buffer.Length);
            }
        }

        protected override void WriteValue(short value)
        {
            var buffer = BitConverter.GetBytes(value);

            if (IsLittleEndian)
            {
                BitHelper.SwapBytes(buffer, 0, BitHelper.ShortSize);
            }

            _stream.Write(buffer, 0, BitHelper.ShortSize);
        }

        protected override void WriteValue(long value)
        {
            var buffer = BitConverter.GetBytes(value);

            if (IsLittleEndian)
            {
                BitHelper.SwapBytes(buffer, 0, BitHelper.LongSize);
            }

            _stream.Write(buffer, 0, BitHelper.LongSize);
        }

        protected override void WriteValue(int[] value)
        {
            if (value != null && value.Length != 0)
            {
                WriteValue(value.Length);
                foreach (var item in value)
                {
                    WriteValue(item);
                }
            }
            else
            {
                WriteValue(0);
            }
        }

        protected override void WriteValue(int value)
        {
            var buffer = BitConverter.GetBytes(value);

            if (IsLittleEndian)
            {
                BitHelper.SwapBytes(buffer, 0, BitHelper.IntSize);
            }

            _stream.Write(buffer, 0, BitHelper.IntSize);
        }

        protected override void WriteValue(float value)
        {
            var buffer = BitConverter.GetBytes(value);

            if (IsLittleEndian)
            {
                BitHelper.SwapBytes(buffer, 0, BitHelper.FloatSize);
            }

            _stream.Write(buffer, 0, BitHelper.FloatSize);
        }

        protected override void WriteValue(double value)
        {
            var buffer = BitConverter.GetBytes(value);

            if (IsLittleEndian)
            {
                BitHelper.SwapBytes(buffer, 0, BitHelper.DoubleSize);
            }

            _stream.Write(buffer, 0, BitHelper.DoubleSize);
        }

        protected override void WriteValue(byte value)
        {
            _stream.WriteByte(value);
        }

        protected override void WriteValue(byte[] value)
        {
            if (value != null && value.Length != 0)
            {
                WriteValue(value.Length);
                _stream.Write(value, 0, value.Length);
            }
            else
            {
                WriteValue(0);
            }
        }

        protected override void WriteValue(TagCollection value)
        {
            _state.StartList(value.LimitType, value.Count);

            _stream.WriteByte((byte) value.LimitType);

            WriteValue(value.Count);

            foreach (var item in value)
            {
                WriteTag(item);
            }
        }

        protected override void WriteValue(TagDictionary value)
        {
            foreach (var item in value)
            {
                WriteTag(item);
            }
        }

        private void WriteEnd()
        {
            _stream.WriteByte((byte) TagType.End);
        }

        #endregion
    }
}