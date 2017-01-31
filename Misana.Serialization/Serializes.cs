using System;

namespace Misana.Serialization
{
    public class Serializes<T>
    {
        public static Action<T, Serializer> Serialize;
        public static Func<Deserializer, T> Deserialize;
    }
}