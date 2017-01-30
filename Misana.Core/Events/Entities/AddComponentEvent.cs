using System;
using System.IO;
using System.Threading.Tasks;
using Misana.Core.Ecs;

namespace Misana.Core.Events.Entities
{
    public class AddComponentEvent<T> : OnEvent where T : Component, new()
    {
        public AddComponentEvent()
        {

        }

        public AddComponentEvent(T template)
        {
            Template = template;
        }

        public readonly T Template;

        public override void Serialize(Version version, BinaryWriter bw)
        {
            //throw new NotImplementedException();
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            //throw new NotImplementedException();
        }

        internal override bool ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, ISimulation world)
        {
            var a = ComponentRegistry<T>.TakeManagedAddition();
            a.EntityId = target.Id;
            a.Template = Template;

            manager.Change(a);
            return true;
        }

        public override OnEvent Copy()
        {
            return new AddComponentEvent<T>(Template);
        }
    }
}