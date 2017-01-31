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

        public override void CopyTo(RenderNameComponent other)
        {
            other.Render = Render;
            other.Text = Text;
            other.Offset = Offset;
        }
    }
}