using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class EntityCollisionRemoverSystem : BaseSystemR1<EntityCollision>
    {
        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Entities[i--].Remove<EntityCollision>();
            }
        }

        protected override void Update(Entity e, EntityCollision r1) { }
    }
}