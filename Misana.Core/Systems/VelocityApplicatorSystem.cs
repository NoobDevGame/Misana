using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class VelocityApplicatorSystem : BaseSystemR2<EntityCollision, VelocityApplicator>
    {
        protected override void Update(Entity e, EntityCollision r1, VelocityApplicator r2)
        {
            var other = Manager.GetEntityById(r1.OtherEntityId);

            if(other == null)
                return;

            var motion = other.Get<MotionComponent>();

            if(motion == null)
                return;

            motion.Move += r2.Force;
        }
    }
}