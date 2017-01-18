using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class WieldedSystem : BaseSystemR3<WieldedComponent, TransformComponent, FacingComponent>
    {
        protected override void Update(Entity e, WieldedComponent r1, TransformComponent r2, FacingComponent r3)
        {
            if(r2.ParentEntityId == 0)
                return;

            var parent = Manager.GetEntityById(r2.ParentEntityId);

            if(parent == null)
                return;

            var parentTransform = parent.Get<TransformComponent>();
            var parentFacing = parent.Get<FacingComponent>();

            if (parentTransform != null)
            {
                r2.CurrentArea = parentTransform.CurrentArea;
                r1.ParentPosition = parentTransform.Position;
            }

            if (parentTransform != null && parentFacing != null)
            {
                r3.Facing = parentFacing.Facing;
                r2.Position = r3.Facing * r1.Distance;
            }

            var parentWielding = parent.Get<WieldingComponent>();
            r1.Use = parentWielding != null && parentWielding.Use;
        }
    }
}