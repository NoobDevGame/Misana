using System;
using System.Text;

namespace Misana.Serialization
{
    public delegate T Deserialize<out T> (byte[] bytes, ref int index);

    public static unsafe class Deserializer
    {
        public static float ReadSingle(byte[] buffer, ref int start)
        {
            fixed (byte* numPtr = buffer)
            {
                var val = *(float*) (numPtr + start);
                start += 4;
                return val;
            }
        }

        public static double ReadDouble(byte[] buffer, ref int start)
        {
            fixed (byte* numPtr = buffer)
            {
                var val = *(double*) (numPtr + start);
                start += 8;
                return val;
            }
        }

        public static long ReadInt64(byte[] buffer, ref int start)
        {
            fixed (byte* numPtr = buffer)
            {
                var val = *(long*) (numPtr + start);
                start += 8;
                return val;
            }
        }

        public static uint ReadUInt32(byte[] buffer, ref int start)
        {
            fixed (byte* numPtr = buffer)
            {
                var val = *(uint*) (numPtr + start);
                start += 4;
                return val;
            }
        }

        public static ulong ReadUInt64(byte[] buffer, ref int start)
        {
            fixed (byte* numPtr = buffer)
            {
                var val = *(ulong*) (numPtr + start);
                start += 8;
                return val;
            }
        }

        public static ushort ReadUInt16(byte[] buffer, ref int start)
        {
            fixed (byte* numPtr = buffer)
            {
                var val = *(ushort*) (numPtr + start);
                start += 2;
                return val;
            }
        }

        public static int ReadInt32(byte[] buffer, ref int start)
        {
            fixed (byte* numPtr = buffer)
            {
                var val = *(int*) (numPtr + start);
                start += 4;
                return val;
            }
        }

        public static bool ReadBoolean(byte[] buffer, ref int start)
        {
            return buffer[start++] == 1;
        }

        public static short ReadInt16(byte[] buffer, ref int start)
        {
            fixed (byte* numPtr = buffer)
            {
                var val = *(short*) (numPtr + start);
                start += 2;
                return val;
            }
        }

        public static byte ReadByte(byte[] buffer, ref int start)
        {
            return buffer[start++];
        }

        public static string ReadString(byte[] buffer, ref int start)
        {
            var len = ReadInt32(buffer, ref start);

            fixed (byte* numPtr = buffer)
            {
                var str = Encoding.UTF8.GetString(numPtr + start, len);
                start += len;
                return str;
            }
        }

        public static void Initialize()
        {
            Serializes<bool>.Deserialize = ReadBoolean;

            Serializes<byte>.Deserialize = ReadByte;
            Serializes<short>.Deserialize = ReadInt16;
            Serializes<int>.Deserialize = ReadInt32;
            Serializes<long>.Deserialize = ReadInt64;

            Serializes<uint>.Deserialize = ReadUInt32;
            Serializes<ushort>.Deserialize = ReadUInt16;
            Serializes<ulong>.Deserialize = ReadUInt64;

            Serializes<float>.Deserialize = ReadSingle;
            Serializes<double>.Deserialize = ReadDouble;

            Serializes<string>.Deserialize = ReadString;

            Serializes<TimeSpan>.Deserialize = (byte[] bytes, ref int index) => TimeSpan.FromTicks(ReadInt64(bytes, ref index));
            Serializes<DateTime>.Deserialize = (byte[] bytes, ref int index) => new DateTime(ReadInt64(bytes, ref index));
        }
    }
}