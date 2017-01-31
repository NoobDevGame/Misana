using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class ProjectileComponent : Component<ProjectileComponent>
    {
        [Copy, Reset]
        public Vector2 Move;
    }
}