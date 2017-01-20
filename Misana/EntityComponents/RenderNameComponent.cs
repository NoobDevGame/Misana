using Misana.Core.Ecs;

namespace Misana.EntityComponents
{
    public class RenderNameComponent : Component<RenderNameComponent>
    {
        public bool Render;
        public string Text;
        public engenious.Vector2 Offset;
        public engenious.Vector2 Position;

        public override void CopyTo(RenderNameComponent other)
        {
            other.Render = Render;
            other.Text = Text;
            other.Offset = Offset;
        }
    }
}