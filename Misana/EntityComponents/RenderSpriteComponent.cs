using engenious;
using engenious.Graphics;
using Misana.Core;
using Misana.Core.Ecs;

namespace Misana.EntityComponents
{
    public class RenderSpriteComponent : Component<RenderSpriteComponent>
    {
        public bool Render; 
        public Texture2D Texture;
        public engenious.Vector2 Center;
        public Index2 TilePosition = new Index2(-1, -1);
        public Rectangle Source;
        public Rectangle Destination;
        public Color Color;

        public override void CopyTo(RenderSpriteComponent other)
        {
            other.Render = Render;
            other.Center = Center;
            //other.TilePosition = TilePosition;
        }
    }
}