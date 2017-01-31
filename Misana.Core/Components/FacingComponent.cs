using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class FacingComponent : Component<FacingComponent>
    {
        [Copy, Reset]
        public Vector2 Facing;
    }
}