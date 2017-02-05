using System;
using System.IO;
using Misana.Core.Ecs;
using Misana.Core.Events.Entities;
using Misana.Core.Events.OnUse;
using Misana.Serialization;

namespace Misana.Core.Events
{
    public abstract class EventDefinition
    {

        public RunsOn RunsOn;
        public int Id { get;}

        public EventDefinition()
        {
            Id = EventIdentifier.NextId();
        }

        public abstract void Serialize(Version version,BinaryWriter bw);
        public abstract void Deserialize(Version version, BinaryReader br);

        public static void Initialize()
        {
            OnEvent.Initialize();
            OnUseEvent.Initialize();
            EventCondition.Initialize();
        }
    }
}