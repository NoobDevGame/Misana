using Misana.Serialization;

namespace Misana.Core
{
    public class MisanaSerializer
    {
        public static void Initialize()
        {
            Serializes<Vector2>.Serialize = (vector2, serializer) => {
                serializer.WriteSingle(vector2.X);
                serializer.WriteSingle(vector2.Y);
            };

            Serializes<Vector2>.Deserialize = (deserializer) => new Vector2(deserializer.ReadSingle(), deserializer.ReadSingle());

            Serializes<Index2>.Serialize = (index2, serializer) => {
                serializer.WriteInt32(index2.X);
                serializer.WriteInt32(index2.Y);
            };

            Serializes<Index2>.Deserialize = (deserializer) 
                => new Index2(deserializer.ReadInt32(), deserializer.ReadInt32());
        }
    }
}