using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Map;

namespace Misana
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MisanaGame game = new MisanaGame())
            {
                var map = MapLoader.CreateMapFromTiled("Lobby", "Lobby/main");

                MapLoader.Save(map);

                game.Run(60, 60);
            }
        }
    }
}
