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
                ListViewItem lvi = new ListViewItem(l.Name);
                lvi.Tag = l;

                lvi.SubItems.Add(l.Id.ToString());

                lvi.Checked = true;

                listView.Items.Add(lvi);

                area = a;
            }
        }

        private int[] GetSelectedIDs()
        {
            if (listView.SelectedItems.Count == 0)
                return null;

            return listView.SelectedItems.Cast<ListViewItem>().Select(t => ((Layer)t.Tag).Id).ToArray();
        }

        private void listView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var ids = GetSelectedIDs();

            if (ids == null)
                return;

            if (HiddenLayers.Contains(ids[0]) && e.NewValue == CheckState.Checked)
                HiddenLayers.Remove(ids[0]);
            else if (!HiddenLayers.Contains(ids[0]) && e.NewValue == CheckState.Unchecked)
                HiddenLayers.Add(ids[0]);

            mainForm.EventBus.Publish(new LayerVisibilityChangedEvent(ids[0]));
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ids = GetSelectedIDs();

            if (ids == null)
                return;

            if (listView.SelectedItems.Count == 0)
                return;

            if (HiddenLayers.Contains((ids[0])))
                HiddenLayers.Remove(ids[0]);

            mainForm.EventBus.Publish(new SelectedLayerChangedEvent(ids[0]));

            
        }

        private void button_addLayer_Click(object sender, EventArgs e)
        {
            if (area == null)
                return;

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

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right && listView.SelectedItems.Count != 0)
            {
                contextMenuStrip_layer.Show(listView.PointToScreen(e.Location));
            }
        }

        private void toolStripMenuItem_rename_Click(object sender, EventArgs e)
        {
            if(listView.SelectedItems.Count == 1)
            {
                listView.SelectedItems[0].BeginEdit();
            }
        }

        private void toolStripMenuItem_remove_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            bool changed = false;
            foreach(ListViewItem lvi in listView.SelectedItems)
            {
                if (lvi.Tag != null)
                {
                    area.Layers.Remove((Layer)lvi.Tag);
                    changed = true;
                }
            }
            mainForm.EventBus.Publish<AreaChangedEvent>(new AreaChangedEvent(area));
        }

        private void listView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            ((Layer)listView.SelectedItems[0].Tag).Name = e.Label;

            mainForm.EventBus.Publish<AreaChangedEvent>(new AreaChangedEvent(area));
        }
    }
}
