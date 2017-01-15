using Misana.Core;
using Misana.Core.Maps;
using Misana.Editor.Events;
using Misana.Editor.Forms.MDI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor.Controls
{
    public class AreaRenderControl : Control
    {
        public bool IsSingleTileSelected
        {
            get
            {
                return (SelectionStart == SelectionEnd);
            }
        }

        public Index3 SelectionStart {
            get
            {
                return selectionStart;
            }
            set
            {
                if (selectionStart == value)
                    return;

                if (value.Z != selectionEnd.Z)
                    throw new NotSupportedException();

                selectionStart = value;

                selectionOnly = true;

                Invalidate();

                mainForm.EventBus.Publish(new MapTileSelectionEvent(selectionStart, selectionEnd));
            }
        }

        public Index3 SelectionEnd
        {
            get
            {
                return selectionEnd;
            }
            set
            {
                if (selectionEnd == value)
                    return;

                if (value.Z != selectionStart.Z)
                    throw new NotSupportedException();

                selectionEnd = value;

                selectionOnly = true;

                Invalidate();

                mainForm.EventBus.Publish(new MapTileSelectionEvent(selectionStart, selectionEnd));
            }
        }

        private Index3 selectionStart;
        private Index3 selectionEnd;

        public Area Area { get; private set; }

        public bool RenderSpawnpoint
        {
            get
            {
                return renderSpawnpoint;
            }

            set
            {
                if (renderSpawnpoint == value)
                    return;

                renderSpawnpoint = value;
                Invalidate();
            }
        }

        private bool renderSpawnpoint = false;

        public AreaMode Mode { get; set; }

        private MainForm mainForm;
        private AreaRenderer areaRenderer;

        private Bitmap bitmapCache;

        private bool selectionOnly = false;

        private Index3 selectionStartBuffer;
        private Index3 selectionEndBuffer;

        private Point lastMousePoint;
        private bool mouseDown;

        private string tilesheetName;
        private int tilesheetTileID;
        private Index2 tilesheetTileIndex;

        public AreaRenderControl(MainForm mainForm, AreaRenderer areaRenderer, Area area)
        {
            this.mainForm = mainForm;
            this.areaRenderer = areaRenderer;

            Area = area;

            Height = area.Height * 32;
            Width = area.Width * 32;

            DoubleBuffered = true;

            var tileControl = mainForm.WindowManager.GetWindow<TilesheetWindow>();
            if (tileControl != null)
            { 
                tilesheetTileID = tileControl.SelectedTileID;
                tilesheetName = tileControl.SelectedTilesheetName;
                tilesheetTileIndex = tileControl.SelectedTileIndex;
            }

            mainForm.EventBus.Subscribe<TilesheetTileSelectionEvent>(TilesheetTileSelectionChanged);

            Select(new Index3(1, 1, 0), new Index3(4, 4, 0));
        }

        private void TilesheetTileSelectionChanged(TilesheetTileSelectionEvent ev)
        {
            tilesheetTileID = ev.TileID;
            tilesheetName = ev.TilesheetName;
            tilesheetTileIndex = ev.TileIndex;
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (mainForm == null || areaRenderer == null)
                return;

            if (mainForm.Map == null || Area == null)
                return;

            if(Width != Area.Width*32 || Height != Area.Height * 32)
            {
                Width = Area.Width * 32;
                Height = Area.Height * 32;
            }

            base.OnInvalidated(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (mainForm == null || areaRenderer == null)
                return;

            if (mainForm.Map == null || Area == null)
                return;

            var g = e.Graphics;

            using (Bitmap bitMap = new Bitmap(Area.Width * 32, Area.Height * 32))
            using (Graphics bg = Graphics.FromImage(bitMap))
            {
                for (int l = 0; l < Area.Layers.Count; l++)
                {
                    var layer = Area.Layers[l];
                    //if (editor.LayerVisibilities[editor.CurrentArea.Layers[l].Id] == false)
                    //    continue;

                    for (int x = 0; x < Area.Width; x++)
                    {
                        for (int y = 0; y < Area.Height; y++)
                        {
                            var index = Area.GetTileIndex(x, y);
                            var tile = layer.Tiles[index];

                            if (tile == null || tile.TilesheetID == 0 || tile.TextureID == 0)
                                continue;

                            var tilesheetName = Area.TilesheetName(tile.TilesheetID);
                            var tileID = tile.TextureID - 1;

                            var sheet = mainForm.TilesheetManager.Tilesheets[tilesheetName];

                            var image = sheet.Texture;

                            var tHeight = (image.Height + 1) / (sheet.TileHeight + sheet.Spacing);
                            var tWidth = (image.Width + 1) / (sheet.TileWidth + sheet.Spacing);
                            var tx = (tileID) % tWidth;
                            var ty = (tileID) / tWidth;

                            bg.DrawImage(image, new Rectangle(x * 32, y * 32, 32, 32), new Rectangle(tx * 17, ty * 17, 16, 16), GraphicsUnit.Pixel);
                        }
                    }
                }

                bitmapCache = (Bitmap)bitMap.Clone();

            }


            g.DrawImage(bitmapCache, new Point(0, 0));


            var selWidth = SelectionEnd.X - SelectionStart.X+1;
            var selHeight = selectionEnd.Y - SelectionStart.Y+1;

            g.DrawRectangle(new Pen(Color.FromArgb(255, 0, 102, 204)), new Rectangle(SelectionStart.X * 32 - 1, SelectionStart.Y * 32 - 1, selWidth*32, selHeight*32));

            if (RenderSpawnpoint)
                g.DrawRectangle(new Pen(Color.Red), new Rectangle((int)Area.SpawnPoint.X * 32 - 1, (int)Area.SpawnPoint.Y * 32 - 1, 32, 32));

            if (Mode == AreaMode.Paint && tilesheetName != null && tilesheetTileIndex != null)
                g.DrawImage(mainForm.TilesheetManager.Tilesheets[tilesheetName].Texture, new Rectangle(lastMousePoint.X/32*32, lastMousePoint.Y/32*32, 32, 32), new Rectangle(tilesheetTileIndex.X *17, tilesheetTileIndex.Y*17,17,17), GraphicsUnit.Pixel);

            selectionOnly = false;

            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDown = true;

            if (Mode == AreaMode.Select)
            {
                selectionStartBuffer = new Index3(e.X / 32, e.Y / 32, 0);
            }

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            lastMousePoint = e.Location;

            if (Mode == AreaMode.Paint && tilesheetName != null && tilesheetTileIndex != null)
            {
                Invalidate();
            }

            if (!mouseDown)
                return;

            if(Mode == AreaMode.Select)
            {
                selectionEndBuffer = new Index3(e.X / 32, e.Y / 32, 0);

                var selStart = selectionStartBuffer;
                var selEnd = selectionEndBuffer;

                if (selectionStartBuffer.X > selectionEndBuffer.X)
                {
                    selStart.X = selectionEndBuffer.X;
                    selEnd.X = selectionStartBuffer.X;
                }

                if (selectionStartBuffer.X > selectionEndBuffer.X)
                {
                    selStart.Y = selectionEndBuffer.Y;
                    selEnd.Y = selectionStartBuffer.Y;
                }

                selectionStart = selStart;
                selectionEnd = selEnd;
                Invalidate();
            }
            else if(Mode == AreaMode.Paint && tilesheetName != null && tilesheetTileIndex != null)
            {
                Index3 tile = new Index3(e.X / 32, e.Y / 32, 0);

                if (!Area.Tilesheets.ContainsValue(tilesheetName))
                    Area.AddTilesheet(tilesheetName);

                var tileIndex = Area.GetTileIndex(tile.X, tile.Y);
                try
                {
                    Area.Layers[tile.Z].Tiles[tileIndex].TextureID = tilesheetTileID;
                    Area.Layers[tile.Z].Tiles[tileIndex].TilesheetID = Area.Tilesheets.FirstOrDefault(t => t.Value == tilesheetName).Key;
                }catch(Exception ex)
                {
                    mainForm.EventBus.Publish(new ErrorEvent("Paint Error", "Could not paint tile " + tileIndex + " on Layer " + tile.Z, false, ex));
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!mouseDown)
                return;

            mouseDown = false;

            if (Mode == AreaMode.Select)
            {
                selectionEndBuffer = new Index3(e.X / 32, e.Y / 32, 0);

                var selStart = selectionStartBuffer;
                var selEnd = selectionEndBuffer;

                if (selectionStartBuffer.X > selectionEndBuffer.X)
                {
                    selStart.X = selectionEndBuffer.X;
                    selEnd.X = selectionStartBuffer.X;
                }

                if (selectionStartBuffer.Y > selectionEndBuffer.Y)
                {
                    selStart.Y = selectionEndBuffer.Y;
                    selEnd.Y = selectionStartBuffer.Y;
                }

                Select(selStart, selEnd);
            }
        }

        public void Select(Index3 start, Index3 end)
        {
            if (start.Z != end.Z)
                throw new NotSupportedException();

            selectionStart = start;
            selectionEnd = end;

            Invalidate();

            mainForm.EventBus.Publish(new MapTileSelectionEvent(selectionStart, selectionEnd));
        }

        public enum AreaMode
        {
            Select,
            Paint,
            Fill
        }
    }
}
