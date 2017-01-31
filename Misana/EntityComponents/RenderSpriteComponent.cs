using engenious;
using engenious.Graphics;
using Misana.Core;
using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.EntityComponents
{
    public class RenderSpriteComponent : Component<RenderSpriteComponent>
    {
        [Copy, Reset]
        public bool Render;
        [Copy, Reset]
        public Texture2D Texture;
        [Copy, Reset]
        public engenious.Vector2 Center;
        [Copy, Reset]
        public Index2 TilePosition = new Index2(-1, -1);
        [Copy, Reset]
        public Rectangle Source;
        [Copy, Reset]
        public Rectangle Destination;
        [Copy, Reset]
        public Color Color;
    }
}