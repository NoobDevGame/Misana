using System;
using System.IO;
using Misana.Core.Ecs;

namespace Misana.Core.Events.Entities
{
    public class CustomCodeEvent : OnEvent
    {
        private readonly Action<EntityManager, Entity, World> _action;

        public CustomCodeEvent(Action<EntityManager, Entity, World> action)
        {
            _action = action;
        }
        

        public override void Serialize(Version version, BinaryWriter bw)
        {
            
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            
        }

        internal override bool ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, World world)
        {
            _action(manager, target, world);
            return true;
        }

        public override OnEvent Copy()
        {
            return new CustomCodeEvent(_action);
        }
    }
}