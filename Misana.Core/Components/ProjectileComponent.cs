using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class ProjectileComponent : Component<ProjectileComponent>
    {
        public override void CopyTo(ProjectileComponent other)
        {
            other.Move = Move;
        }

        public Vector2 Move;
    }
}