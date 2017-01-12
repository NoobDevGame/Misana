using System.Runtime.InteropServices;
using engenious;
using engenious.Graphics;

namespace Misana.Controls
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MapVertex: IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;
        static MapVertex()
        {
            VertexDeclaration = new VertexDeclaration(new VertexElement(0,VertexElementFormat.Single,VertexElementUsage.Position,0));
        }
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public MapVertex(Vector2 position, Vector2 texturePosition, int textureId)
        {
            PackedData = (uint)(((uint) position.X & 0xFF) << 24 | ((uint) position.Y & 0xFF) << 16 | (textureId & 0x3FFF) << 2 |
                         ((uint) texturePosition.X & 0x1)<<1 | ((uint) texturePosition.Y & 0x1));
        }
        public uint PackedData { get; private set; }
    }
}
