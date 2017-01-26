using System;
using System.IO;
using System.Threading.Tasks;
using Misana.Core.Ecs;

namespace Misana.Core.Events.Entities
{
    public class CustomCodeEvent : OnEvent
    {
        private readonly Action<EntityManager, Entity, ISimulation> _action;

        public CustomCodeEvent(Action<EntityManager, Entity, ISimulation> action)
        {
            _action = action;
        }
        

        public override void Serialize(Version version, BinaryWriter bw)
        {
            
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            
        }

        internal override async Task<bool> ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, ISimulation world)
        {
            _action(manager, target, world);
            return await  Task.FromResult(true);
        }

        public override OnEvent Copy()
        {
            return new CustomCodeEvent(_action);
        }
    }
}