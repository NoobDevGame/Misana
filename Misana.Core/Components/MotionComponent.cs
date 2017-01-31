using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class MotionComponent : Component<MotionComponent>
    {
        [Copy, Reset]
        public Vector2 Move;

        public override void CopyTo(MotionComponent other)
        {
            other.Move = Move;
        }
    }
}
