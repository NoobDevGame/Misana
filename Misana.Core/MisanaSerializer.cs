using Misana.Serialization;

namespace Misana.Core
{
    public class MisanaSerializer
    {
        public static void Initialize()
        {
            Serializes<Vector2>.Serialize = (Vector2 item, ref byte[] bytes, ref int index) => {
                Serializer.WriteSingle(item.X, ref bytes, ref index);
                Serializer.WriteSingle(item.Y, ref bytes, ref index);
            };

            Serializes<Vector2>.Deserialize  = (byte[] bytes, ref int index) =>
                new Vector2(Deserializer.ReadSingle(bytes, ref index), Deserializer.ReadSingle(bytes, ref index));

            Serializes<Index2>.Serialize = (Index2 item, ref byte[] bytes, ref int index) => {
                Serializer.WriteInt32(item.X, ref bytes, ref index);
                Serializer.WriteInt32(item.Y, ref bytes, ref index);
            };

            Serializes<Index2>.Deserialize  = (byte[] bytes, ref int index) =>
                new Index2(Deserializer.ReadInt32(bytes, ref index), Deserializer.ReadInt32(bytes, ref index));
        }
    }
}