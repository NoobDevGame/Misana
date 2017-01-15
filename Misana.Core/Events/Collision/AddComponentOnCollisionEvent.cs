using System;
using System.IO;
using Misana.Core.Ecs;

namespace Misana.Core.Events.Collision
{
    public class AddComponentOnCollisionEvent<T> : OnCollisionEvent where T : Component, new()
    {
        public AddComponentOnCollisionEvent()
        {

        }

        public AddComponentOnCollisionEvent(T template)
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

        internal override bool ApplyToEntity(EntityManager manager, bool targetIsSelf, Entity target, World world)
        {
            var a = ComponentRegistry<T>.TakeManagedAddition();
            a.EntityId = target.Id;
            a.Template = Template;

            manager.Change(a);
            return true;
        }
    }
}