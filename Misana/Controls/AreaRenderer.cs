using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using engenious.Graphics;
using Misana.Components;
using Misana.Core.Maps;
using System.Collections.Generic;

namespace Misana.Controls
{
    class AreaRenderer : IDisposable
    {
        private LayerRenderer[] _layerRenderer;
        public Area Area { get; private set; }
        private ScreenComponent _screen;
        private Texture2DArray _tiles;

        private Dictionary<string, Tilesheet> tilesheets;

        Dictionary<int, int> offsets = new Dictionary<int, int>();

        public AreaRenderer(ScreenComponent screen,Area area, Dictionary<string,Tilesheet> tilesheets)
        {
            this.tilesheets = tilesheets;
            Area = area;
            _screen = screen;
            RebuildTiles();
            RebuildRenderer();
        }

        private void RebuildTiles()
        {
            if (Area.Tilesheets.Count == 0)
                return;

            offsets.Clear();


            int tileCount = 0;
            int width =-1, height = -1;
            foreach (var set in Area.Tilesheets)
            {
                var tile = tilesheets[set.Value];
                if (width != -1 && (tile.TileWidth != width || tile.TileHeight != height))
                    throw new NotSupportedException("non uniform tile sizes not supported");
                width = tile.TileWidth;
                height = tile.TileHeight;
                offsets.Add(set.Key, tileCount);
                tileCount += tile.TileCount;
            }
            _tiles = new Texture2DArray(_screen.GraphicsDevice, 1, width, height, tileCount);
            int tileIndex = 0;


            int[] tileBuffer = new int[width * height];
            foreach (var set in Area.Tilesheets)
            {
                var tile = tilesheets[set.Value];
                int x = 0, y = 0;
                var text = _screen.Content.Load<Texture2D>(tile.TextureName);
                Debug.WriteLine(set.Key);
                int[] buffer = new int[text.Width * text.Height];
                text.GetData(buffer);


                int curColumn=0;
                for (int currentTile=0;currentTile<tile.TileCount;currentTile++)
                {
                    int yOffset = 0;
                    int curWidth = Math.Min(text.Width - x, width);

                    if (curWidth != width)
                        Array.Clear(tileBuffer,0,tileBuffer.Length);
                    for (int i = 0; i < text.Width * height; i += text.Width, yOffset++)
                    {
                        Array.Copy(buffer, (y * text.Width + x) + i, tileBuffer, yOffset * width + (width-curWidth)/2, curWidth);
                    }


                    _tiles.SetData(tileBuffer, tileIndex++);

                    x += width + tile.Spacing;
                    curColumn++;
                    if (x >= text.Width/* || curColumn >= tile.Columns*/)
                    {
                        y += height + tile.Spacing;
                        x = 0;
                        curColumn = 0;
                    }
                }

            }
        }

        private void RebuildRenderer()
        {
            var layers = Area.Layers.ToArray();
            _layerRenderer = new LayerRenderer[layers.Length];
            for (int index = 0; index < layers.Length; index++)
            {
                _layerRenderer[index] = new LayerRenderer(_screen,Area.Width, Area.Height, layers[index],_tiles,offsets);
            }
        }

        public void Render(Effect effect)
        {
            effect.Parameters["TileTextures"].SetValue(_tiles);
            _tiles.SamplerState = SamplerState.LinearClamp;
            for (int index = 0; index < _layerRenderer.Length; index++)
            {
                _layerRenderer[index].Render();
            }
        }

        public void Dispose()
        {
            foreach(var layer in _layerRenderer)
            {
                layer.Dispose();
            }
            _tiles?.Dispose();
        }
    }
}
