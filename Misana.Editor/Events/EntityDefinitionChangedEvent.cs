using Misana.Core.Entities;
using Redbus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Events
{
    public class EntityDefinitionChangedEvent : EventBase
    {
        public EntityDefinition EntityDefinition { get; set; }

        public EntityDefinitionChangedEvent(EntityDefinition ed)
        {
            EntityDefinition = ed;
        }
    }
}
