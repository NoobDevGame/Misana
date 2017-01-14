using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Components
{
    public class TransformComponent : Component<TransformComponent>
    {
        public Area CurrentArea;
        public Vector2 Position;

        public float Radius = 0.5f;

        public Vector2 Size => new Vector2(2 * Radius, 2 * Radius);
        public Vector2 HalfSize => new Vector2(Radius, Radius);
    }
}