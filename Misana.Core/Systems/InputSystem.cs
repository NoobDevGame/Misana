using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Systems
{
    [SystemConfiguration]
    public class InputSystem : BaseSystemR2<PlayerInputComponent, MotionComponent>
    {
        public InputSystem(EntityManager manager) : base(manager)
        {
        }

        protected override void Update(Entity e, PlayerInputComponent r1, MotionComponent r2)
        {
            r2.Move = r1.Move * GameTime.ElapsedTime.TotalSeconds;
        }
    }
}
