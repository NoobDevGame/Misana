using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Maps
{
    public struct Tile
    {
        public int TextureID;
        public int TilesheetID;
        public bool Blocked;

        public Tile(int textureID, int tilesheetID, bool blocked)
        {
            TextureID = textureID;
            TilesheetID = tilesheetID;
            Blocked = blocked;
        }

    }
}
