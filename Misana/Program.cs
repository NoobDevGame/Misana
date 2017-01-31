using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using Misana.Core;
using Misana.Core.Ecs;
using Misana.Core.Maps;
using Misana.Serialization;

namespace Misana
{
    class Program
    {
        static void Main(string[] args)
        {
            Serializer.Initialize();
            Deserializer.Initialize();

            Serializes<engenious.Vector2>.Serialize = (vector2, serializer) => {
                serializer.WriteSingle(vector2.X);
                serializer.WriteSingle(vector2.Y);
            };

            Serializes<engenious.Vector2>.Deserialize = (deserializer) => new engenious.Vector2(deserializer.ReadSingle(), deserializer.ReadSingle());

            EntityManager.Initialize();
            MisanaSerializer.Initialize();

            using (MisanaGame game = new MisanaGame())
            {
                game.Run(60, 60);
            }
        }
    }
}
