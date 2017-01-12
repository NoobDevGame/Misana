using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class CollisionApplicatorSystem : BaseSystemR2<EntityCollision,CollisionApplicator>
    {
        protected override void Update(Entity e, EntityCollision r1, CollisionApplicator r2)
        {
            foreach (var entityId in r1.OtherEntityIds)
            {
                var otherEntity = Manager.GetEntityById(entityId);
                if (otherEntity != null)
                    r2.Action?.Invoke(otherEntity);

            }
        }
    }
}