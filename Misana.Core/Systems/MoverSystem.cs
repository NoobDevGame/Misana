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
    public class MoverSystem : BaseSystemR2<MotionComponent, PositionComponent>
    {
        public MoverSystem(EntityManager manager) : base(manager)
        {
        }

        protected override void Update(Entity e, MotionComponent r1, PositionComponent r2)
        {
            r2.Position += r1.Move;
        }
    }
}
