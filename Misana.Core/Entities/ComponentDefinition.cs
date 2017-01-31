using System;
using System.IO;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities
{
    public abstract class ComponentDefinition
    {
        public RunsOn RunsOn;

        public abstract void ApplyDefinition(EntityBuilder entity,Map map, ISimulation sim);

        public virtual void Serialize(Version version,BinaryWriter bw)
        {

        }

        public virtual void Deserialize(Version version,BinaryReader br)
        {

        }


    }

    public abstract class ComponentDefinition<T> : ComponentDefinition
         where T : Component<T> , new ()
    {
        public override void ApplyDefinition(EntityBuilder entity, Map map, ISimulation sim)
        {
            entity.Add<T>(c => {
                c.RunsOn = RunsOn;
                OnApplyDefinition(entity, map, c, sim);
            });
        }

        public abstract void OnApplyDefinition(EntityBuilder entity,Map map, T component, ISimulation sim);
    }
}