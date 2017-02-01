using Misana.Core;
using Misana.Core.Maps;
using Misana.Editor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor.Forms.MDI
{
    public partial class AreaRenderer : Control
    {
        public Area Area { get { return areaRenderControl.Area; } }

        public Index2[] SelectedTiles { get { return areaRenderControl.SelectedTiles; }}
        public int? SelectedLayer { get { return areaRenderControl.SelectedLayer; } }

        public bool IsSingleTileSelected { get { return areaRenderControl.IsSingleTileSelected; } }

        private Application app;

        private AreaRenderControl areaRenderControl;
        
        public AreaRenderer(Application mainForm, Area area)
        {
            InitializeComponent();

            this.app = mainForm;

            areaRenderControl = new AreaRenderControl(mainForm,this,area);
            control_panel.AutoScroll = true;
            control_panel.Controls.Add(areaRenderControl);
        }
        internal void ModeSelectionChanged(object sender, EventArgs e)
        {
            if(button_mode_select.Checked == true)
            {
                areaRenderControl.Mode = AreaRenderControl.AreaMode.Select;
            }
            else if(button_mode_paint.Checked == true)
            {
                areaRenderControl.Mode = AreaRenderControl.AreaMode.Paint;
            }
            else if(button_mode_fill.Checked == true)
            {
                areaRenderControl.Mode = AreaRenderControl.AreaMode.Fill;
            }
        }

        private void button_toggle_spawnpoint_CheckStateChanged(object sender, EventArgs e)
        {
            areaRenderControl.RenderSpawnpoint = button_toggle_spawnpoint.Checked;
        }

        private void button_toggle_spawnpoint_Click(object sender, EventArgs e)
        {
            if(IsSingleTileSelected)
            {
                areaRenderControl.Area.SpawnPoint = new Vector2(SelectedTiles[0].X + 0.5f, SelectedTiles[0].Y + 0.5f);
            }
        }
    }
}
