using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class ProjectileSystem : BaseSystemR2<ProjectileComponent, MotionComponent>
    {
        protected override void Update(Entity e, ProjectileComponent r1, MotionComponent r2)
        {
            r2.Move = r1.Move;
        }
    }
}