using Misana.Core.Maps;
using Misana.Editor.Models;
using Redbus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Events
{
    public class MapChangedEvent : EventBase
    {
        public MapModel Map { get; set; }

        public MapChangedEvent(MapModel map)
        {
            Map = map;
        }
    }
}
