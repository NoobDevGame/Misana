using Misana.Core;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities;
using Misana.Core.Entities.BaseDefinition;
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
                return SelectedTiles.Length == 1;
            }
        }

        public Index2[] SelectedTiles { get { return selectedTiles.ToArray(); } }

        public AreaEntity[] SelectedEntities { get { return selectedEntities.ToArray(); } }

        public int? SelectedLayer { get { return selectedLayer; } }

        private List<Index2> selectedTiles = new List<Index2>();
        private List<AreaEntity> selectedEntities = new List<AreaEntity>();
        private int selectedLayer;

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

        private Index2 selectionStartBuffer;
        private Index2 selectionEndBuffer;

        private Index2 tempSelectionStart;
        private Index2 tempSelectionEnd;

        private Point lastMousePoint;
        private bool mouseDown;

        private string tilesheetName;
        private int tilesheetTileID;
        private Index2 tilesheetTileIndex;

        public AreaRenderControl(MainForm mainForm, AreaRenderer areaRenderer, Area area)
        {
            this.mainForm = mainForm;
            this.areaRenderer = areaRenderer;

            AllowDrop = true;

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

            mainForm.EventBus.Subscribe<SelectedLayerChangedEvent>((e) => {
                selectedLayer = e.LayerID;
                Invalidate();
              });

            mainForm.EventBus.Subscribe<LayerVisibilityChangedEvent>((e) => Invalidate());
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
            if (e.Graphics == null)
                return;

            if (mainForm == null || areaRenderer == null)
                return;

            if (mainForm.Map == null || Area == null)
                return;

            var g = e.Graphics;

            var hiddenLayers = mainForm.WindowManager.GetWindow<LayerView>().HiddenLayers;

            using (Bitmap bitMap = new Bitmap(Area.Width * 32, Area.Height * 32))
            using (Graphics bg = Graphics.FromImage(bitMap))
            {
                bg.FillRectangle(new SolidBrush(Color.CornflowerBlue), new Rectangle(0, 0, bitMap.Width, bitMap.Height));

                for (int l = 0; l < Area.Layers.Count; l++)
                {
                    if (hiddenLayers.Contains(Area.Layers[l].Id))
                        continue;

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

                    if(SelectedLayer != null && SelectedLayer == Area.Layers[l].Id)
                    {
                        if (Mode == AreaMode.Paint && tilesheetName != null && tilesheetTileIndex != null)
                            bg.DrawImage(mainForm.TilesheetManager.Tilesheets[tilesheetName].Texture, new Rectangle(lastMousePoint.X / 32 * 32, lastMousePoint.Y / 32 * 32, 32, 32), new Rectangle(tilesheetTileIndex.X * 17, tilesheetTileIndex.Y * 17, 16, 16), GraphicsUnit.Pixel);
                    }

                   

                }

                bitmapCache = (Bitmap)bitMap.Clone();

            }


            g.DrawImage(bitmapCache, new Point(0, 0));

            foreach (var eD in Area.Entities)
            {
                var transformDefintion = (TransformDefinition)eD.Definition.Definitions.FirstOrDefault(t => t.GetType() == typeof(TransformDefinition));
                if (transformDefintion == null)
                    continue;

                g.FillEllipse(new SolidBrush(Color.Red), new Rectangle((int)transformDefintion.Position.X * 32, (int)transformDefintion.Position.Y * 32, 32, 32));
            }


            //var selWidth = SelectionEnd.X - SelectionStart.X+1;
            //var selHeight = selectionEnd.Y - SelectionStart.Y+1;

            //g.DrawRectangle(new Pen(Color.FromArgb(255, 0, 102, 204)), new Rectangle(SelectionStart.X * 32 - 1, SelectionStart.Y * 32 - 1, selWidth*32, selHeight*32));

            if (Mode == AreaMode.Select)
            {
                foreach (var tile in SelectedTiles)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Blue)), new Rectangle(tile.X * 32, tile.Y * 32, 32, 32));
                }

                if(mouseDown)
                {
                    var selWidth = tempSelectionEnd.X - tempSelectionStart.X + 1;
                    var selHeight = tempSelectionEnd.Y - tempSelectionStart.Y + 1;
                    g.DrawRectangle(new Pen(Color.Blue), new Rectangle(tempSelectionStart.X * 32, tempSelectionStart.Y  * 32, selWidth * 32, selHeight * 32));
                }
            }

            if (RenderSpawnpoint)
                g.DrawRectangle(new Pen(Color.Red), new Rectangle((int)Area.SpawnPoint.X * 32 - 1, (int)Area.SpawnPoint.Y * 32 - 1, 32, 32));

            selectionOnly = false;

            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDown = true;

            if (Mode == AreaMode.Select)
            {
                selectionStartBuffer = new Index2(e.X / 32, e.Y / 32);
                tempSelectionStart = selectionStartBuffer;
            }
            else if(Mode == AreaMode.Fill && SelectedLayer != null && tilesheetName != null && tilesheetTileIndex != null)
            {
                if (!Area.Tilesheets.ContainsValue(tilesheetName))
                    Area.AddTilesheet(tilesheetName);

                foreach(var t in Area.Layers[(int)SelectedLayer].Tiles)
                {
                    t.TilesheetID = Area.Tilesheets.FirstOrDefault(x => x.Value == tilesheetName).Key;
                    t.TextureID = tilesheetTileID;
                }
                Invalidate();
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
                selectionEndBuffer = new Index2(e.X / 32, e.Y / 32);

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

                tempSelectionStart = selStart;
                tempSelectionEnd = selEnd;
               
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
                    if (SelectedLayer != null)
                    {
                        Area.Layers[(int)SelectedLayer].Tiles[tileIndex].TextureID = tilesheetTileID;
                        Area.Layers[(int)SelectedLayer].Tiles[tileIndex].TilesheetID = Area.Tilesheets.FirstOrDefault(t => t.Value == tilesheetName).Key;
                    }
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
                if ((ModifierKeys.HasFlag(Keys.Control) || ModifierKeys.HasFlag(Keys.Shift)))
                    Select(tempSelectionStart, tempSelectionEnd, true);
                else
                    Select(tempSelectionStart, tempSelectionEnd, false);
            }
        }

        public void Select(Index2 start, Index2 end, bool keepSelection = false)
        {
            if (!keepSelection)
            {
                selectedTiles.Clear();
                selectedEntities.Clear();
            }

            start.X = Math.Max(0, Math.Min(start.X, Area.Width - 1));
            start.Y = Math.Max(0, Math.Min(start.Y, Area.Height - 1));
            end.X = Math.Max(0, Math.Min(end.X, Area.Width - 1));
            end.Y = Math.Max(0, Math.Min(end.Y, Area.Height - 1));

            for (int x = start.X; x <= end.X; x++)
            {
                for (int y = start.Y; y <= end.Y; y++)
                {
                    var tiles = SelectedTiles.Where(t => t.X == x && t.Y == y);
                    if (tiles.Count() == 0)
                        selectedTiles.Add(new Index2(x, y));

                    
                    foreach(var en in Area.Entities)
                    {
                        var transform = (TransformDefinition)en.Definition.Definitions.FirstOrDefault(t => t.GetType() == typeof(TransformDefinition));
                        if (transform != null && (int) transform.Position.X == x && (int)transform.Position.Y == y && !selectedEntities.Contains(en))
                            selectedEntities.Add(en); 
                    }
                }
            }

            Invalidate();

            Tile[] sTiles = new Tile[SelectedTiles.Length];
            var count = 0;
            if (SelectedLayer != null)
            {
                foreach (var ind2 in SelectedTiles)
                {
                    var tileIndex = Area.GetTileIndex(ind2.X, ind2.Y);
                    sTiles[count] = Area.Layers[(int)SelectedLayer].Tiles[tileIndex];
                    count++;
                }

                mainForm.EventBus.Publish(new MapTileSelectionEvent((int)SelectedLayer,sTiles.ToArray(),SelectedTiles));
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            drgevent.Effect = DragDropEffects.Copy;
            base.OnDragEnter(drgevent);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            var edef = (EntityDefinition)drgevent.Data.GetData(typeof(EntityDefinition));

            if (edef == null)
                return;

            //var x = (drgevent.X-this.ClientRectangle.X) / 32;
            //var y = (drgevent.Y-this.ClientRectangle.Y) / 32;

            var ptc = PointToClient(new Point(drgevent.X, drgevent.Y));
            var x = ptc.X / 32;
            var y = ptc.Y / 32;

            var ndef = edef.Copy();//new EntityDefinition(edef.Name);

            //foreach (var xdef in edef.Definitions)
            //    ndef.Definitions.Add(xdef);

            var z = (TransformDefinition) ndef.Definitions.FirstOrDefault(b => b.GetType() == typeof(TransformDefinition));

            if (z == null)
            {
                ndef.Definitions.Add(new TransformDefinition());
                z = (TransformDefinition)ndef.Definitions.FirstOrDefault(b => b.GetType() == typeof(TransformDefinition));
            }

            z.AreaId = Area.Id;
            z.Position = new Vector2(x+0.5f, y+0.5f);
            z.Radius = 0.9f;

            Area.Entities.Add(new AreaEntity() { Name = new Random().Next().ToString(), Definition =  ndef });

            Invalidate();

            base.OnDragDrop(drgevent);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                if(selectedEntities.Count > 0)
                {
                    Area.Entities.RemoveAll(t => selectedEntities.Contains(t));
                    selectedEntities.Clear();
                }
            }
            base.OnKeyDown(e);
        }

        public enum AreaMode
        {
            Select,
            Paint,
            Fill
        }
    }
}
