using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Maps
{
    public class Tile
    {
        public int TextureID;
        public int TilesheetID;
        public bool Blocked;

        public bool blocked { get { return Blocked; } set { Blocked = value; } }

        public Tile(int textureID, int tilesheetID, bool blocked)
        {
            TextureID = textureID;
            TilesheetID = tilesheetID;
            Blocked = blocked;
        }

    }
}
