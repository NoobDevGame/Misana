using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class ProjectileComponent : Component<ProjectileComponent>
    {
        public override void CopyTo(ProjectileComponent other)
        {
            other.Move = Move;
        }
        [Copy, Reset]
        public Vector2 Move;
    }
}