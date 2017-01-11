using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Misana.Core;

namespace Misana.Editor.Controls
{
    public partial class RenderControl : UserControl
    {
        private Editor editor;

        private bool selectionOnly = false;

        private Bitmap bitmapCache;

        public Index2 SelectedTile { get
            {
                return selectedTile;
            }
            set
            {
                if (selectedTile == value) //TODO ==
                    return;

                selectedTile = value;
                //Invalidate(new Rectangle(selectedTile.X * 32 - 1, selectedTile.Y * 32 - 1, 33,33));
                selectionOnly = true;
                Invalidate();
            }
        } 

        private Index2 selectedTile = new Index2(1,1);

        public RenderControl(Editor editor)
        {
            this.editor = editor;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            
            if (editor == null)
                return;

            if(editor.Map == null || editor.CurrentArea == null)
                return;

            var g = e.Graphics;

            if (!selectionOnly)
            {
                using (Bitmap bitMap = new Bitmap(editor.CurrentArea.Width * 32, editor.CurrentArea.Height * 32))
                using (Graphics bg = Graphics.FromImage(bitMap))
                {
                    for (int l = 0; l < editor.CurrentArea.Layers.Length; l++)
                    {
                        var layer = editor.CurrentArea.Layers[l];

                        for (int x = 0; x < editor.CurrentArea.Width; x++)
                        {
                            for (int y = 0; y < editor.CurrentArea.Height; y++)
                            {

                                var index = x + editor.CurrentArea.Width * y;
                                var id = layer.Tiles[index];

                                if (id == 0)
                                    continue;

                                var maptexture = editor.CurrentArea.GetMapTextures(id);
                                var image = editor.TileSheets[maptexture.Key];

                                var rawid = id - maptexture.Firstgid;
                                var rx = rawid % maptexture.Columns;
                                var ry = rawid / maptexture.Columns;

                                bg.DrawImage(image, new Rectangle(x * 32, y * 32, 32, 32), new Rectangle(rx * 17, ry * 17, 16, 16), GraphicsUnit.Pixel);
                            }
                        }
                    }

                    bitmapCache = (Bitmap)bitMap.Clone();


                }
            }


            g.DrawImage(bitmapCache, new Point(0, 0));



            g.DrawRectangle(new Pen(Color.FromArgb(255, 0, 102, 204)), new Rectangle(SelectedTile.X * 32 -1, SelectedTile.Y * 32 -1, 32, 32));

            selectionOnly = false;

            base.OnPaint(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int x = e.X / 32;
            int y = e.Y / 32;
            SelectedTile = new Index2(x, y);
            base.OnMouseClick(e);
        }
    }
}
