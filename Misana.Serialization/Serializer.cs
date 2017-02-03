using System;
using System.IO;
using System.Text;

namespace Misana.Serialization
{
    public delegate void Serialize<in T> (T item, ref byte[] bytes, ref int index);

    public static unsafe class Serializer
    {
        public static void EnsureSize(ref byte[] buffer, int len)
        {
            if (buffer.Length < len)
            {
                var tmp = new byte[buffer.Length * 2 < len ? len : buffer.Length * 2];
                Array.Copy(buffer, tmp, buffer.Length);
                buffer = tmp;
            }
        }
        public static void WriteSingle(float i, ref byte[] buffer, ref int index)
        {
            fixed (byte* numPtr = buffer)
                *(int*) (numPtr + index) = *(int*) &i;

            index += 4;
        }

        public static void WriteDouble(double i, ref byte[] buffer, ref int index)
        {
            fixed (byte* numPtr = buffer)
                *(long*) (numPtr + index) = *(long*) &i;
            index += 8;
        }

        public static void WriteInt64(long i, ref byte[] buffer, ref int index)
        {
            fixed (byte* numPtr = buffer)
                *(long*) (numPtr + index) = i;
            index += 8;
        }

        public static void WriteUInt32(uint i, ref byte[] buffer, ref int index)
        {
            fixed (byte* numPtr = buffer)
                *(uint*) (numPtr + index) = i;
            index += 4;
        }

        public static void WriteUInt64(ulong i, ref byte[] buffer, ref int index)
        {
            fixed (byte* numPtr = buffer)
                *(ulong*) (numPtr + index) = i;
            index += 8;
        }

        public static void WriteUInt16(ushort i, ref byte[] buffer, ref int index)
        {
            fixed (byte* numPtr = buffer)
                *(ushort*) (numPtr + index) = i;
            index += 2;
        }

        public static void WriteInt32(int i, ref byte[] buffer, ref int index)
        {
            fixed (byte* numPtr = buffer)
                *(int*) (numPtr + index) = i;

            index += 4;
        }

        public static void WriteBoolean(bool i, ref byte[] buffer, ref int index)
        {
            buffer[index++] = (byte) (i ? 1 : 0);
        }

        public static void WriteInt16(short i, ref byte[] buffer, ref int index)
        {
            fixed (byte* numPtr = buffer)
                *(short*) (numPtr + index) = i;

            index += 2;
        }

        public static void  WriteByte(byte b, ref byte[] buffer, ref int index)
        {
            buffer[index++] = b;
        }

        public static void  WriteString(string s, ref byte[] buffer, ref int index)
        {
            var len = Encoding.UTF8.GetByteCount(s);

            EnsureSize(ref buffer, index + 4 + len + 1);

            WriteInt32(len, ref buffer, ref index);

            Encoding.UTF8.GetBytes(s, 0, s.Length, buffer, index);
            index += len;
        }
        

        public static void Initialize()
        {
            Serializes<bool>.Serialize = WriteBoolean;

            Serializes<byte>.Serialize = WriteByte;
            Serializes<short>.Serialize = WriteInt16;
            Serializes<int>.Serialize = WriteInt32;
            Serializes<long>.Serialize = WriteInt64;

            Serializes<uint>.Serialize = WriteUInt32;
            Serializes<ushort>.Serialize = WriteUInt16;
            Serializes<ulong>.Serialize = WriteUInt64;

            Serializes<float>.Serialize = WriteSingle;
            Serializes<double>.Serialize = WriteDouble;

            Serializes<string>.Serialize = WriteString;

            Serializes<TimeSpan>.Serialize = (TimeSpan item, ref byte[] bytes, ref int index) => WriteInt64(item.Ticks, ref bytes, ref index);
            Serializes<DateTime>.Serialize = (DateTime item, ref byte[] bytes, ref int index) => WriteInt64(item.Ticks, ref bytes, ref index);
        }
    }
}