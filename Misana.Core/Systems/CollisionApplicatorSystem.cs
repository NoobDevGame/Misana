using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class CollisionApplicatorSystem : BaseSystemR2<EntityCollisionComponent,CollisionApplicator>
    {
        private World world;

        public CollisionApplicatorSystem(World world)
        {
            this.world = world;
        }

        protected override void Update(Entity e, EntityCollisionComponent r1, CollisionApplicator r2)
        {
            foreach (var entityId in r1.OtherEntityIds)
            {
                if(entityId == e.Id)
                    continue;

                var otherEntity = Manager.GetEntityById(entityId);
                if (otherEntity != null)
                    r2.Action?.Invoke(otherEntity,world);

            }
        }


    }
}