using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Components
{
    public class MotionComponent : Component<MotionComponent>
    {
        public Vector2 Move;

        public override void CopyTo(MotionComponent other)
        {
            other.Move = Move;
        }
    }
}
