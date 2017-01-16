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
using WeifenLuo.WinFormsUI.Docking;

namespace Misana.Editor.Forms.MDI
{
    public partial class AreaRenderer : DockContent, IMDIForm
    {
        public DockState DefaultDockState => DockState.Document;

        public Area Area { get { return areaRenderControl.Area; } }

        public Index2[] SelectedTiles { get { return areaRenderControl.SelectedTiles; }}
        public int? SelectedLayer { get { return areaRenderControl.SelectedLayer; } }

        public bool IsSingleTileSelected { get { return areaRenderControl.IsSingleTileSelected; } }

        private MainForm mainForm;

        private AreaRenderControl areaRenderControl;
        
        public AreaRenderer(MainForm mainForm, Area area)
        {
            InitializeComponent();

            this.mainForm = mainForm;

            AutoScroll = true;

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
        }

        private void button_toggle_spawnpoint_CheckStateChanged(object sender, EventArgs e)
        {
            areaRenderControl.RenderSpawnpoint = button_toggle_spawnpoint.Checked;
        }
    }
}
