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

            var parent = r2.Parent(Manager);

            if(parent == null)
                return;

            var parentTransform = parent.Get<TransformComponent>();
            var parentFacing = parent.Get<FacingComponent>();

            if (parentTransform != null)
            {
                r2.CurrentAreaId = parentTransform.CurrentAreaId;

                if (parentTransform.ParentEntityId != 0)
                {
                    var pw = parent.Get<WieldedComponent>();
                    if (pw != null)
                    {
                        r1.ParentPosition = parentTransform.Position + pw.ParentPosition;
                        
                    }
                    else
                    {
                        r1.ParentPosition = parentTransform.Position;
                    }
                }
                else
                {
                    r1.ParentPosition = parentTransform.Position;
                }
                
            }

            if (parentFacing != null)
                r1.ParentFacing = parentFacing.Facing;

            if (parentTransform != null && parentFacing != null)
            {
                r3.Facing = parentFacing.Facing;
                r2.Position = r3.Facing * r1.Offset;
            }

            var parentWielding = parent.Get<WieldingComponent>();
            r1.Use = parentWielding != null && parentWielding.Use;

            var w = e.Get<WieldingComponent>();
            if (w != null)
            {
                w.Use = r1.Use;
            }

        }
    }
}