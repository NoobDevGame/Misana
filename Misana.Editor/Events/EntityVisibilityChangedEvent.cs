using Redbus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Events
{
    public class EntityVisibilityChangedEvent : EventBase
    {
        public bool Visible { get; set; }
        public EntityVisibilityChangedEvent(bool visible)
        {
            Visible = visible;
        }
    }
}
