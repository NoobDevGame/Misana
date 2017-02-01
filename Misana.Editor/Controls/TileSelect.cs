using Misana.Core;
using Misana.Core.Maps;
using Misana.Editor;
using Misana.Editor.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Misana.Editor.Controls
{

    public partial class TileSelect : Control
    {
        private Application app;

        private TabControl tabControl;

        public KeyValuePair<string, Index2> Selection
        {
            get
            {
                return selection;
            }
            set
            {
                if (Selection.Key == value.Key && Selection.Value == value.Value)
                    return;

                selection = new KeyValuePair<string, Index2>(value.Key, value.Value);
                ChangeSelection(selection.Key, selection.Value);
                app.EventBus.Publish(new TilesheetTileSelectionEvent(SelectedTileId, selection.Key, selection.Value));
            }
        }

        public int SelectedTileId
        {
            get
            {
                if (Selection.Key == null)
                    return 0;
                var ts = app.TilesheetManager.Tilesheets[Selection.Key];

                return (Selection.Value.Y) * ts.Columns + Selection.Value.X + 1;
            }
        }

        private KeyValuePair<string, Index2> selection;

        private Dictionary<string, TilesheetRenderer> tilesheetRenderer = new Dictionary<string, TilesheetRenderer>();

        public TileSelect(Application mainForm)
        {
            this.app = mainForm;

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            Controls.Add(tabControl);

            RenderTilesheets();
        }

        private void RenderTilesheets()
        {
            foreach (Tilesheet sheet in app.TilesheetManager.Tilesheets.Values)
            {
                TabPage p = new TabPage(sheet.Name);

                TilesheetRenderer tr = new TilesheetRenderer(sheet);
                tr.Anchor = AnchorStyles.Left | AnchorStyles.Top;

                tr.OnSelectionChanged += (s, e) =>
                {
                    if (Selection.Key != null && tilesheetRenderer[Selection.Key] == tr && tr.Selection == Selection.Value)
                        return;

                    var myKey = tilesheetRenderer.FirstOrDefault(x => x.Value == tr).Key;

                    Selection = new KeyValuePair<string, Index2>(myKey, tr.Selection);
                };

                p.AutoScroll = true;

                tilesheetRenderer.Add(sheet.Name, tr);

                p.Controls.Add(tr);
                tabControl.TabPages.Add(p);

            }
        }

        private class InternalTilesheetTileSelectionEvent
        {
            public TilesheetRenderer TilesheetRenderer { get; set; }
            public InternalTilesheetTileSelectionEvent(TilesheetRenderer tr)
            {
                TilesheetRenderer = tr;
            }
        }

        private class TilesheetRenderer : UserControl
        {
            private Tilesheet tilesheet;

            public Index2 Selection
            {
                get
                {
                    return selection;
                }
                set
                {
                    if (selection == value)
                        return;

                    selection = value;
                    Invalidate();

                    OnSelectionChanged?.Invoke(this,null);
                }
            }

            public event EventHandler OnSelectionChanged;

            public Brush SelectorBrush { get; set; } = new SolidBrush(Color.Blue);

            public bool Active { get; set; }

            private Index2 selection = new Index2(-1, -1);

            public TilesheetRenderer(Tilesheet tilesheet)
            {
                this.tilesheet = tilesheet;
                this.Height = tilesheet.Texture.Height *2;
                this.Width = tilesheet.Texture.Width *2;

                DoubleBuffered = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (e.Graphics == null)
                    return;

                e.Graphics.DrawImage(tilesheet.Texture, new Rectangle(0, 0, tilesheet.Texture.Width * 2, tilesheet.Texture.Height * 2));

                if (Selection == null || Active == false)
                    return;

                var selectorWidth = (tilesheet.TileWidth) * 2;
                var selectorHeight = (tilesheet.TileHeight) * 2;

                Pen pen = new Pen(SelectorBrush);
                e.Graphics.DrawRectangle(pen, Selection.X * (selectorWidth + 2), Selection.Y * (selectorHeight + 2), selectorWidth, selectorHeight);

                base.OnPaint(e);
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);
                Selection = new Index2(e.X / ((tilesheet.TileWidth + tilesheet.Spacing) * 2), e.Y / ((tilesheet.TileHeight + tilesheet.Spacing) * 2));
            }
        }

        private void ChangeSelection(string tilesheetname, Index2 position)
        {
            if (!app.TilesheetManager.Tilesheets.ContainsKey(tilesheetname))
                return;

            foreach (TilesheetRenderer sr in tilesheetRenderer.Values)
                sr.Active = false;

            var s = tilesheetRenderer.FirstOrDefault(t => t.Key == tilesheetname);

            s.Value.Selection = position;
            s.Value.Active = true;
        }
    }
}