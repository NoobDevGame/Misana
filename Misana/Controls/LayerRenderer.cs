using System;
using System.Collections.Generic;
using engenious;
using engenious.Graphics;
using Misana.Components;
using Misana.Core.Maps;

namespace Misana.Controls
{
    class LayerRenderer : IDisposable
    {
        private VertexBuffer vb;
        private ScreenComponent _screen;
        private Texture2DArray _tiles;
        public LayerRenderer(ScreenComponent screen, int width, int height, Layer layer,Texture2DArray tiles)
        {
            _tiles = tiles;
            _screen = screen;
            List<MapVertex> vertices = new List<MapVertex>(width*height*4);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = layer.Tiles[x + y * width];

                    if (tile.TextureID == 0 || tile.TilesheetID == 0)
                        continue;

                    vertices.Add(new MapVertex(new Vector2(x, y), new Vector2(0, 0), (byte)tile.TextureID));
                    vertices.Add(new MapVertex(new Vector2(x + 1, y), new Vector2(1, 0), (byte)tile.TextureID));
                    vertices.Add(new MapVertex(new Vector2(x, y + 1), new Vector2(0, 1), (byte)tile.TextureID));
                    vertices.Add(new MapVertex(new Vector2(x + 1, y + 1), new Vector2(1, 1), (byte)tile.TextureID));
                }
            }
            vb = new VertexBuffer(screen.GraphicsDevice, MapVertex.VertexDeclaration, vertices.Count);
            vb.SetData(vertices.ToArray());
        }

        public void Render()
        {
            _screen.GraphicsDevice.VertexBuffer = vb;
            _screen.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles,0,0,vb.VertexCount,0,vb.VertexCount/2);
        }
        public void Dispose()
        {
            vb?.Dispose();
        }
    }
}