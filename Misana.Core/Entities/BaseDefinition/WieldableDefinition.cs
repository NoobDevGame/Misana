using System;
using System.Collections.Generic;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Events.Entities;
using Misana.Core.Events.OnUse;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class WieldableDefinition : ComponentDefinition<WieldableComponent>
    {
        public List<OnUseEvent> OnUseEvents = new List<OnUseEvent>();

        public override void OnApplyDefinition(EntityBuilder entity, Map map, WieldableComponent component)
        {
            component.OnUseEvents = new List<OnUseEvent>(OnUseEvents);
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {

            bw.Write(OnUseEvents.Count);

            foreach (var @event in OnUseEvents)
            {
                bw.Write(@event.GetType().AssemblyQualifiedName);
                @event.Serialize(version, bw);
            }
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var typeName = br.ReadString();
                var @event = (OnUseEvent)Activator.CreateInstance(Type.GetType(typeName));
                @event.Deserialize(version, br);
                OnUseEvents.Add(@event);
            }
        }
    }
}