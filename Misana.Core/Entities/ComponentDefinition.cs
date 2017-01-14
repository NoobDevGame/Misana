using System;
using System.IO;
using Misana.Core.Ecs;

namespace Misana.Core.Entities
{
    public abstract class ComponentDefinition
    {
        public abstract void ApplyDefinition(Entity entity);

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
        public override void ApplyDefinition(Entity entity)
        {
            var component = new T();

            entity.Add<T>(c => OnApplyDefinition(entity,c));
        }

        public abstract void OnApplyDefinition(Entity entity, T component);
    }
}