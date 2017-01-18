using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class FacingComponent : Component<FacingComponent>
    {
        public Vector2 Facing;

        public override void CopyTo(FacingComponent other)
        {
            other.Facing = Facing;
        }
    }
}