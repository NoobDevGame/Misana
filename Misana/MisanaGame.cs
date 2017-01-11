using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Map;

namespace Misana
{
    class MisanaGame : Game
    {
        public Map TestMap { get; private set; }

        public MisanaGame()
        {

        }

        public override void LoadContent()
        {
            TestMap = MapLoader.Load("Lobby");
        }
    }
}
