using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class FacingComponent : Component<FacingComponent>
    {
        [Copy, Reset]
        public Vector2 Facing;

        public override void CopyTo(FacingComponent other)
        {
            other.Facing = Facing;
        }
    }
}