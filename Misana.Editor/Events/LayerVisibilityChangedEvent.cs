using Redbus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Events
{
    public class LayerVisibilityChangedEvent : EventBase
    {
        public int LayerID { get; set; }
        
        public LayerVisibilityChangedEvent(int id)
        {
            LayerID = id;
        }
    }
}
