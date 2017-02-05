using Misana.Core;
using Misana.Core.Ecs;
using Misana.Serialization;
using Vector2 = engenious.Vector2;

namespace Misana
{
    class Program
    {
        static void Main(string[] args)
        {
            Serializer.Initialize();
            Deserializer.Initialize();

            Serializes<engenious.Vector2>.Serialize  = (Vector2 item, ref byte[] bytes, ref int index) => {
                Serializer.WriteSingle(item.X, ref bytes, ref index);
                Serializer.WriteSingle(item.Y, ref bytes, ref index);
            };

            Serializes<engenious.Vector2>.Deserialize  = (byte[] bytes, ref int index)
                =>  new engenious.Vector2(Deserializer.ReadSingle(bytes, ref index), Deserializer.ReadSingle(bytes, ref index));

            MisanaSerializer.Initialize();
            EntityManager.Initialize();

            using (MisanaGame game = new MisanaGame())
            {
                game.Run(60, 60);
            }
        }
    }
}
