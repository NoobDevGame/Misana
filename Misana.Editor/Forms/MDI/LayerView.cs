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

        public LayerView(MainForm mainForm) : base()
        {
            this.mainForm = mainForm;
            InitializeComponent();

            mainForm.EventBus.Subscribe<AreaSelectionEvent>(AreaSelectionEvent);

            listView.HideSelection = false;

            mainForm.VSToolStripExtender.SetStyle(toolStrip1, VisualStudioToolStripExtender.VsVersion.Vs2015, mainForm.Theme);
        }

        public void AreaSelectionEvent(AreaSelectionEvent ev)
        {
            listView.Items.Clear();

            foreach(Layer l in ev.Area.Layers)
            {
                ListViewItem lvi = new ListViewItem(l.Id.ToString());

                lvi.Tag = l.Id;

                lvi.SubItems.Add(l.Name);

                lvi.Checked = true;

                listView.Items.Add(lvi);
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
            if (listView.SelectedItems.Count == 0 && listView.Items.Count > 0)
            {
                listView.Items[0].Selected = true;
                Invalidate();
            }

            if (HiddenLayers.Contains((int)listView.SelectedItems[0].Tag))
                HiddenLayers.Remove((int)listView.SelectedItems[0].Tag);

            mainForm.EventBus.Publish(new SelectedLayerChangedEvent((int)listView.SelectedItems[0].Tag));


        }
    }
}
