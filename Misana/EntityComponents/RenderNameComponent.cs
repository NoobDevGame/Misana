using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.EntityComponents
{
    public class RenderNameComponent : Component<RenderNameComponent>
    {
        [Copy, Reset]
        public bool Render;
        [Copy, Reset]
        public string Text;
        [Copy, Reset]
        public engenious.Vector2 Offset;
        [Copy, Reset]
        public engenious.Vector2 Position;
    }
}