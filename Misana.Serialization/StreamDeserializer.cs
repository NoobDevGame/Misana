using System.IO;
using System.Text;

namespace Misana.Serialization
{
    public unsafe class StreamDeserializer : Deserializer
    {
        private readonly byte[] _buffer = new byte[8192];
        private readonly Stream _stream;

        public StreamDeserializer(Stream stream)
        {
            _stream = stream;
        }

        public override float ReadSingle()
        {
            _stream.Read(_buffer, 0, 4);

            fixed (byte* numPtr = _buffer)
                return *(float*) numPtr;
        }

        public override double ReadDouble()
        {
            _stream.Read(_buffer, 0, 8);

            fixed (byte* numPtr = _buffer)
                return *(double*) numPtr;
        }

        public override long ReadInt64()
        {
            _stream.Read(_buffer, 0, 8);

            fixed (byte* numPtr = _buffer)
                return *(long*) numPtr;
        }

        public override uint ReadUInt32()
        {
            _stream.Read(_buffer, 0, 4);

            fixed (byte* numPtr = _buffer)
                return *(uint*) numPtr;
        }

        public override ulong ReadUInt64()
        {
            _stream.Read(_buffer, 0, 8);

            fixed (byte* numPtr = _buffer)
                return *(ulong*) numPtr;
        }

        public override ushort ReadUInt16()
        {
            _stream.Read(_buffer, 0, 2);

            fixed (byte* numPtr = _buffer)
                return *(ushort*) numPtr;
        }

        public override int ReadInt32()
        {
            _stream.Read(_buffer, 0, 4);

            fixed (byte* numPtr = _buffer)
                return *(int*) numPtr;
        }

        public override bool ReadBoolean()
        {
            return ReadByte() == 1;
        }

        public override short ReadInt16()
        {
            _stream.Read(_buffer, 0, 2);

            fixed (byte* numPtr = _buffer)
                return *(short*) numPtr;
        }

        public override byte ReadByte()
        {
            _stream.Read(_buffer, 0, 1);
            return _buffer[0];
        }

        public override string ReadString()
        {
            var len = ReadInt32();

            fixed (byte* b = _buffer)
                return Encoding.UTF8.GetString(b, len);
        }
    }
}