using Misana.Core.Maps;
using Redbus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Events
{
    public class AreaAddEvent : EventBase
    {
        public Area Area { get; set; }

        public AreaAddEvent(Area area)
        {
            Area = area;
        }
    }
}
