using System;

namespace Misana.Serialization
{
    public abstract class Deserializer
    {
        public abstract float ReadSingle();
        public abstract double ReadDouble();
        public abstract long ReadInt64();
        public abstract uint ReadUInt32();
        public abstract ulong ReadUInt64();
        public abstract ushort ReadUInt16();
        public abstract int ReadInt32();
        public abstract bool ReadBoolean();
        public abstract short ReadInt16();
        public abstract byte ReadByte();
        public abstract string ReadString();

        public static void Initialize()
        {
            Serializes<bool>.Deserialize = s => s.ReadBoolean();

            Serializes<byte>.Deserialize = s => s.ReadByte();
            Serializes<short>.Deserialize = s => s.ReadInt16();
            Serializes<int>.Deserialize = s => s.ReadInt32();
            Serializes<long>.Deserialize = s => s.ReadInt64();

            Serializes<uint>.Deserialize = s => s.ReadUInt32();
            Serializes<ushort>.Deserialize = s => s.ReadUInt16();
            Serializes<ulong>.Deserialize = s => s.ReadUInt64();

            Serializes<float>.Deserialize = s => s.ReadSingle();
            Serializes<double>.Deserialize = s => s.ReadDouble();

            Serializes<string>.Deserialize = s => s.ReadString();

            Serializes<TimeSpan>.Deserialize = s => TimeSpan.FromTicks(s.ReadInt64());
            Serializes<DateTime>.Deserialize = s => new DateTime(s.ReadInt64());
        }
    }
}