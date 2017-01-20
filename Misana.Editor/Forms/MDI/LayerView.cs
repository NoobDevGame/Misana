using Misana.Core.Maps;
using Misana.Editor.Events;
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
    public partial class LayerView : SingleInstanceDockWindow, IMDIForm
    {
        public DockState DefaultDockState => DockState.DockLeft;

        MainForm mainForm;

        public List<int> HiddenLayers { get; private set; } = new List<int>();

        public int SelectedLayer { get; private set; }

        private Area area;

        public LayerView(MainForm mainForm) : base()
        {
            this.mainForm = mainForm;
            InitializeComponent();

            mainForm.EventBus.Subscribe<AreaSelectionEvent>(t => UpdateArea(t.Area));

            listView.HideSelection = false;

            mainForm.VSToolStripExtender.SetStyle(toolStrip1, VisualStudioToolStripExtender.VsVersion.Vs2015, mainForm.Theme);

            mainForm.EventBus.Subscribe<AreaChangedEvent>(t => UpdateArea(t.Area));
        }

        public void UpdateArea(Area a)
        {
            listView.Items.Clear();

            foreach (Layer l in a.Layers)
            {
                ListViewItem lvi = new ListViewItem(l.Id.ToString());

                lvi.Tag = l.Id;

                lvi.SubItems.Add(l.Name);

                lvi.Checked = true;

                listView.Items.Add(lvi);

                area = a;
            }
        }

        private void listView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (HiddenLayers.Contains((int)listView.Items[e.Index].Tag) && e.NewValue == CheckState.Checked)
                HiddenLayers.Remove((int)listView.Items[e.Index].Tag);
            else if (!HiddenLayers.Contains((int)listView.Items[e.Index].Tag) && e.NewValue == CheckState.Unchecked)
                HiddenLayers.Add((int)listView.Items[e.Index].Tag);

            mainForm.EventBus.Publish(new LayerVisibilityChangedEvent((int)listView.Items[e.Index].Tag));
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            if (HiddenLayers.Contains((int)listView.SelectedItems[0].Tag))
                HiddenLayers.Remove((int)listView.SelectedItems[0].Tag);

            mainForm.EventBus.Publish(new SelectedLayerChangedEvent((int)listView.SelectedItems[0].Tag));

            
        }

        private void button_addLayer_Click(object sender, EventArgs e)
        {
            var maxId = 0;
            if(area.Layers.Count > 0)
                maxId = area.Layers.Max(t => t.Id);

            var tx = new Tile[area.Width * area.Height];

            for (int i = 0; i < tx.Length; i++)
                tx[i] = new Tile(0, 0, false);
            Layer l = new Layer(maxId + 1, "Layer " + (maxId + 1),tx );
            area.Layers.Add(l);

            mainForm.EventBus.Publish(new AreaChangedEvent(area));
        }

        private void toolStripButton_toggleEntities_CheckedChanged(object sender, EventArgs e)
        {
            mainForm.EventBus.Publish(new EntityVisibilityChangedEvent(toolStripButton_toggleEntities.Checked));
        }
    }
}
