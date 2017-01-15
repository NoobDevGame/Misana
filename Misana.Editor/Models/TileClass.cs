using Misana.Core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor
{
    class TileClass
    {
        public int TextureID { get; set; }
        public int SheetID { get; set; }
        public bool Blocked { get; set; }

        public static TileClass FromStruct(Tile tile)
        {
            TileClass t = new TileClass()
            {
                TextureID = tile.TextureID,
                SheetID = tile.TilesheetID,
                Blocked = tile.Blocked
            };
            return t;
        }
    }
}
