using engenious;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;
using Vector2 = Misana.Core.Vector2;

namespace Misana.EntityComponents
{
    public class RenderHealthComponent : Component<RenderHealthComponent>
    {
        [Copy, Reset]
        public bool Render;
        [Copy, Reset]
        public Vector2 Offset;

        [Reset]
        public Rectangle FillRect;
        [Reset]
        public Rectangle Background;
    }
}