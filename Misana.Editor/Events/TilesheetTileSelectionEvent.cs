using Misana.Core;
using Redbus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Events
{
    public class TilesheetTileSelectionEvent : EventBase
    {
        public int TileID { get; set; }
        public string TilesheetName { get; set; }
        public Index2 TileIndex { get; set; }

        public TilesheetTileSelectionEvent(int id, string name, Index2 index)
        {
            TileID = id;
            TilesheetName = name;
            TileIndex = index;
        }
    }
}
