using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Systems
{
    public class InputSystem : BaseSystemR2O1<PlayerInputComponent, MotionComponent, EntityInteractableComponent>
    {
        protected override void Update(Entity e, PlayerInputComponent r1, MotionComponent r2, EntityInteractableComponent o1)
        {
            r2.Move += r1.Move * GameTime.ElapsedTime.TotalSeconds;
            if (o1 != null)
                o1.Interacting = r1.Interact;
        }
    }
}
