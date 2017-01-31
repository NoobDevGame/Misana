using System.IO;
using System.Text;

namespace Misana.Serialization
{
    public unsafe class StreamSerializer : Serializer
    {
        private readonly byte[] _buffer = new byte[8192];
        private readonly Stream _stream;

        public StreamSerializer(Stream stream)
        {
            _stream = stream;
        }

        public override void WriteSingle(float i)
        {
            fixed (byte* numPtr = _buffer)
                *(int*) numPtr = *(int*) &i;

            _stream.Write(_buffer, 0, 4);
        }

        public override void WriteDouble(double i)
        {
            fixed (byte* numPtr = _buffer)
                *(long*) numPtr = *(long*) &i;
        }

        public override void WriteInt64(long i)
        {
            fixed (byte* numPtr = _buffer)
                *(long*) numPtr = *&i;
        }

        public override void WriteUInt32(uint i)
        {
            fixed (byte* numPtr = _buffer)
                *(uint*) numPtr = *&i;
        }

        public override void WriteUInt64(ulong i)
        {
            fixed (byte* numPtr = _buffer)
                *(ulong*) numPtr = *&i;
        }

        public override void WriteUInt16(ushort i)
        {
            fixed (byte* numPtr = _buffer)
                *(ushort*) numPtr = *&i;
        }

        public override void WriteInt32(int i)
        {
            fixed (byte* numPtr = _buffer)
                *(int*) numPtr = *&i;

            _stream.Write(_buffer, 0, 4);
        }

        public override void WriteBoolean(bool i)
        {
            WriteByte((byte) (i ? 1 : 0));
        }

        public override void WriteInt16(short i)
        {
            fixed (byte* numPtr = _buffer)
                *(short*) numPtr = *&i;

            _stream.Write(_buffer, 0, 2);
        }

        public override void WriteByte(byte b)
        {
            _buffer[0] = b;
            _stream.Write(_buffer, 0, 1);
        }

        public override void WriteString(string s)
        {
            var len = Encoding.UTF8.GetByteCount(s);
            WriteInt32(len);
            Encoding.UTF8.GetBytes(s, 0, s.Length, _buffer, 0);
            _stream.Write(_buffer, 0, len);
        }
    }
}