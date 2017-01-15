using Redbus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Events
{
    public class SelectedLayerChangedEvent : EventBase
    {
        public int LayerID { get; set; }

        public SelectedLayerChangedEvent(int id)
        {
            LayerID = id;
        }
    }
}
