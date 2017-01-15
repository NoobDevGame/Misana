using System;
using System.Collections.Generic;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Events;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class CollisionDefinition :ComponentDefinition<CollisionApplicator>
    {
        public List<EventDefinition> EventsActions { get; set; } = new List<EventDefinition>();

        public override void OnApplyDefinition(Entity entity, Map map, CollisionApplicator component)
        {
            if (EventsActions.Count > 0)

            foreach (var @event in EventsActions)
            {
                component.Action += @event.Apply;
            }
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(EventsActions.Count);

            foreach (var @event in EventsActions)
            {
                bw.Write(@event.GetType().AssemblyQualifiedName);
                @event.Serialize(version,bw);
            }
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            var count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var typeName = br.ReadString();
                var @event = (EventDefinition)Activator.CreateInstance(Type.GetType(typeName));
                @event.Deserialize(version,br);
                EventsActions.Add(@event);
            }
        }
    }
}