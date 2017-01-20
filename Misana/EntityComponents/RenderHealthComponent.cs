using engenious;
using Misana.Core.Ecs;
using Vector2 = Misana.Core.Vector2;

namespace Misana.EntityComponents
{
    public class RenderHealthComponent : Component<RenderHealthComponent>
    {
        public bool Render;
        public Vector2 Offset;
        public Rectangle FillRect;
        public Rectangle Background;

        public override void CopyTo(RenderHealthComponent other)
        {
            other.Render = Render;
            other.Offset = Offset;
        }
    }
}