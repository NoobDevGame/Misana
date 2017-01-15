using Misana.Core.Maps;
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
        public Map Map { get; set; }

        public MapChangedEvent(Map map)
        {
            Map = map;
        }
    }
}
