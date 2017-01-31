using System.Text;

namespace Misana.Serialization
{
    public unsafe class ArrayDeserializer : Deserializer
    {
        private readonly byte[] _buffer;
        private int _index;

        public ArrayDeserializer(byte[] arr)
        {
            _buffer = arr;
        }

        public override float ReadSingle()
        {
            fixed (byte* numPtr = _buffer)
            {
                var val = *(float*) (numPtr + _index);
                _index += 4;
                return val;
            }
        }

        public override double ReadDouble()
        {
            fixed (byte* numPtr = _buffer)
            {
                var val = *(double*) (numPtr + _index);
                _index += 8;
                return val;
            }
        }

        public override long ReadInt64()
        {
            fixed (byte* numPtr = _buffer)
            {
                var val = *(long*) (numPtr + _index);
                _index += 8;
                return val;
            }
        }

        public override uint ReadUInt32()
        {
            fixed (byte* numPtr = _buffer)
            {
                var val = *(uint*) (numPtr + _index);
                _index += 4;
                return val;
            }
        }

        public override ulong ReadUInt64()
        {
            fixed (byte* numPtr = _buffer)
            {
                var val = *(ulong*) (numPtr + _index);
                _index += 8;
                return val;
            }
        }

        public override ushort ReadUInt16()
        {
            fixed (byte* numPtr = _buffer)
            {
                var val = *(ushort*) (numPtr + _index);
                _index += 2;
                return val;
            }
        }

        public override int ReadInt32()
        {
            fixed (byte* numPtr = _buffer)
            {
                var val = *(int*) (numPtr + _index);
                _index += 4;
                return val;
            }
        }

        public override bool ReadBoolean()
        {
            return ReadByte() == 1;
        }

        public override short ReadInt16()
        {
            fixed (byte* numPtr = _buffer)
            {
                var val = *(short*) (numPtr + _index);
                _index += 2;
                return val;
            }
        }

        public override byte ReadByte()
        {
            return _buffer[_index++];
        }

        public override string ReadString()
        {
            var len = ReadInt32();

            fixed (byte* numPtr = _buffer)
            {
                var str = Encoding.UTF8.GetString(numPtr, len);
                _index += len;
                return str;
            }
        }
    }
}