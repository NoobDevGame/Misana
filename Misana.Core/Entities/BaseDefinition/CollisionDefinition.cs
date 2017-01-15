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
        public EventCondition Condition { get; set; }

        public List<EventDefinition> EventsActions { get; set; } = new List<EventDefinition>();

        public CollisionDefinition()
        {

        }

        public CollisionDefinition(EventCondition condition)
        {
            Condition = condition;
        }

        public override void OnApplyDefinition(Entity entity, Map map, CollisionApplicator component)
        {
            if (Condition != null)
            {
                component.Condition = Condition.Test;
            }


            foreach (var @event in EventsActions)
            {
                component.Action += @event.Apply;
            }
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {

            bw.Write(Condition != null);
            if (Condition != null)
            {
                bw.Write(Condition.GetType().AssemblyQualifiedName);
                Condition.Serialize(version,bw);
            }

            bw.Write(EventsActions.Count);

            foreach (var @event in EventsActions)
            {
                bw.Write(@event.GetType().AssemblyQualifiedName);
                @event.Serialize(version,bw);
            }
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            var conditionExist = br.ReadBoolean();
            if (conditionExist)
            {
                var typeName = br.ReadString();
                var condition = (EventCondition)Activator.CreateInstance(Type.GetType(typeName));
                condition.Deserialize(version,br);
                Condition = condition;
            }

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