using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class MoverSystem : BaseSystemR2<TransformComponent, MotionComponent>
    {
        protected override void Update(Entity e, TransformComponent r1, MotionComponent r2)
        {
            r1.Position += r2.Move;
            r2.Move = Vector2.Zero;
        }
    }
}