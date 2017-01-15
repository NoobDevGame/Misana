using System;
using System.Collections.Generic;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Events.Collision;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class EntityColliderDefinition : ComponentDefinition<EntityColliderComponent>
    {
        public bool Blocked { get; set; } = true;
        public bool Fixed { get; set; } = false;

        public List<OnCollisionEvent> OnCollisionEvents= new List<OnCollisionEvent>();

        public float Mass { get; set; } = 50f;

        public override void OnApplyDefinition(EntityBuilder entity, Map map, EntityColliderComponent component)
        {
            component.Blocked = Blocked;
            component.Fixed = Fixed;
            component.Mass = Mass;
            component.OnCollisionEvents = new List<OnCollisionEvent>(OnCollisionEvents);
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Mass);
            bw.Write(Blocked);
            bw.Write(Fixed);

            bw.Write(OnCollisionEvents.Count);

            foreach (var @event in OnCollisionEvents)
            {
                bw.Write(@event.GetType().AssemblyQualifiedName);
                @event.Serialize(version, bw);
            }
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Mass = br.ReadSingle();
            Blocked = br.ReadBoolean();
            Fixed = br.ReadBoolean();

            var count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var typeName = br.ReadString();
                var @event = (OnCollisionEvent)Activator.CreateInstance(Type.GetType(typeName));
                @event.Deserialize(version, br);
                OnCollisionEvents.Add(@event);
            }
        }
    }
}