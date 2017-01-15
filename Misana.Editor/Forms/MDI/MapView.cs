using Misana.Core;
using Misana.Core.Maps;
using Misana.Editor.Events;
using Misana.Editor.Forms.Standalone;
using Redbus;
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
    public partial class MapView : DockContent
    {
        private MainForm mainForm;

        private List<SubscriptionToken> subscriptionTokens = new List<SubscriptionToken>();

        public MapView(MainForm mainForm)
        {
            InitializeComponent();

            this.mainForm = mainForm;

            var token = mainForm.EventBus.Subscribe<MapChangedEvent>(MainForm_MapChanged);
            subscriptionTokens.Add(token);

            mainForm.EventBus.Subscribe<AreaAddEvent>((a) => RebuildTree());

            treeView.ImageList = IconHelper.ImageList;

            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
            

        }

        private void TreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(treeView.SelectedNode.Tag is Area)
                mainForm.EventBus.Publish(new AreaSelectionEvent((Area)treeView.SelectedNode.Tag));
        }

        private void MainForm_MapChanged(MapChangedEvent ev)
        {
            RebuildTree();
        }

        private void RebuildTree()
        {
            treeView.Nodes.Clear();
            TreeNode mapNode = new TreeNode(mainForm.Map.Name);
            mapNode.ImageKey = "Map";
            mapNode.SelectedImageKey = "Map";
            foreach (var area in mainForm.Map.Areas)
            {
                TreeNode areaNode = new TreeNode(area.Name);
                areaNode.ImageKey = "Area";
                areaNode.SelectedImageKey = "Area";
                areaNode.Tag = area;
                mapNode.Nodes.Add(areaNode);
            }
            treeView.Nodes.Add(mapNode);
            treeView.ExpandAll();
        }

        private void toolStripButton_addArea_Click(object sender, EventArgs e)
        {
            var a = new NewAreaForm();
            if (a.ShowDialog() == DialogResult.OK)
            {
                Area area = new Area(a.AreaName, mainForm.Map.Areas.Count + 1, a.AreaWidth, a.AreaHeight);
                area.SpawnPoint = new Vector2(1, 1);

                var tiles = new Tile[area.Height * area.Width];

                for (int i = 0; i < tiles.Length; i++)
                    tiles[i] = new Tile(0, 0, false);

                Layer l = new Layer(0, tiles);

                area.Layers.Add(l);
                mainForm.Map.Areas.Add(area);

                mainForm.EventBus.Publish(new AreaAddEvent(area));
            }

        }
    }
}
