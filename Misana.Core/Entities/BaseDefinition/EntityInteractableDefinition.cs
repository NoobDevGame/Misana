using System;
using System.Collections.Generic;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Events.Entities;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class EntityInteractableDefinition : ComponentDefinition<EntityInteractableComponent>
    {
        public List<OnEvent> OnInteractEvents= new List<OnEvent>();

        public override void OnApplyDefinition(EntityBuilder entity, Map map, EntityInteractableComponent component)
        {
            component.OnInteractionEvents = new List<OnEvent>( OnInteractEvents);
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(OnInteractEvents.Count);

            foreach (var @event in OnInteractEvents)
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
                var @event = (OnEvent)Activator.CreateInstance(Type.GetType(typeName));
                @event.Deserialize(version, br);
                OnInteractEvents.Add(@event);
            }
        }
    }
}