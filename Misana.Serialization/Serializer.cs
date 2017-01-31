using System;

namespace Misana.Serialization
{
    public abstract class Serializer
    {
        public abstract void WriteSingle(float i);
        public abstract void WriteDouble(double i);
        public abstract void WriteInt64(long i);
        public abstract void WriteUInt32(uint i);
        public abstract void WriteUInt64(ulong i);
        public abstract void WriteUInt16(ushort i);
        public abstract void WriteInt32(int i);
        public abstract void WriteBoolean(bool i);
        public abstract void WriteInt16(short n);
        public abstract void WriteByte(byte b);
        public abstract void WriteString(string s);

        public static void Initialize()
        {
            Serializes<bool>.Serialize = (i, s) => s.WriteBoolean(i);

            Serializes<byte>.Serialize = (i, s) => s.WriteByte(i);
            Serializes<short>.Serialize = (i, s) => s.WriteInt16(i);
            Serializes<int>.Serialize = (i, s) => s.WriteInt32(i);
            Serializes<long>.Serialize = (i, s) => s.WriteInt64(i);

            Serializes<uint>.Serialize = (i, s) => s.WriteUInt32(i);
            Serializes<ushort>.Serialize = (i, s) => s.WriteUInt16(i);
            Serializes<ulong>.Serialize = (i, s) => s.WriteUInt64(i);

            Serializes<float>.Serialize = (i, s) => s.WriteSingle(i);
            Serializes<double>.Serialize = (i, s) => s.WriteDouble(i);

            Serializes<string>.Serialize = (i, s) => s.WriteString(i);

            Serializes<TimeSpan>.Serialize = (i, s) => s.WriteInt64(i.Ticks);
            Serializes<DateTime>.Serialize = (i, s) => s.WriteInt64(i.Ticks);
        }
    }
}