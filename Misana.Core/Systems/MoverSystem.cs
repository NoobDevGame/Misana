using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class MoverSystem : BaseSystemR2<PositionComponent, MotionComponent>
    {
        protected override void Update(Entity e, PositionComponent r1, MotionComponent r2)
        {
            r1.Position += r2.Move;
            r2.Move = Vector2.Zero;
        }
    }
}