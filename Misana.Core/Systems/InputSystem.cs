using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Systems
{
    public class InputSystem : BaseSystemR3O1<PlayerInputComponent, MotionComponent, TransformComponent, FacingComponent>
    {
        protected override void Update(Entity e, PlayerInputComponent r1, MotionComponent r2, TransformComponent r3, FacingComponent o1)
        {
            r2.Move += r1.Move * GameTime.ElapsedTime.TotalSeconds;

            if (o1 != null)
                o1.Facing = (r3.Position - r1.Facing).Normalize();

            var wielding = e.Get<WieldingComponent>();
            if (wielding != null)
                wielding.Use = r1.Attacking;

            var interacting = e.Get<EntityInteractableComponent>();

            if (interacting != null)
                interacting.Interacting = r1.Interact;
        }
    }
}
