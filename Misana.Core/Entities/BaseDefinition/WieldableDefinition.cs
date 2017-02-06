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
        public Vector2 Offset;

        public WieldableDefinition(float xOffset, float yOffset)
        {
            Offset = new Vector2(xOffset, yOffset);
        }
        public WieldableDefinition(){}

        public override void OnApplyDefinition(EntityBuilder entity, Map map, WieldableComponent component, ISimulation sim)
        {
            component.OnUseEvents = new List<OnUseEvent>(OnUseEvents);
            component.Offset = Offset;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {

            bw.Write(Offset.X);
            bw.Write(Offset.Y);
            bw.Write(OnUseEvents.Count);

            foreach (var @event in OnUseEvents)
            {
                bw.Write(@event.GetType().AssemblyQualifiedName);
                @event.Serialize(version, bw);
            }
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Offset = new Vector2(br.ReadSingle(), br.ReadSingle());
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