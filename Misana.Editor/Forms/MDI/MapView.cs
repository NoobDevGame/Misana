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

namespace Misana.Editor.Forms.MDI
{
    public partial class MapView : Control
    {
        private Application app;

        private List<SubscriptionToken> subscriptionTokens = new List<SubscriptionToken>();

        public MapView(Application mainForm) : base()
        {
            InitializeComponent();

            this.app = mainForm;

            var token = mainForm.EventBus.Subscribe<MapChangedEvent>(MainForm_MapChanged);
            subscriptionTokens.Add(token);

            mainForm.EventBus.Subscribe<AreaAddEvent>((a) => RebuildTree());

            treeView.ImageList = IconHelper.ImageList;

            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
        }

        private void TreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeView.SelectedNode == null)
                return;

            if (treeView.SelectedNode.Tag is Area)
            {
                app.WindowManager.AddControl(new AreaRenderer(app, (Area)treeView.SelectedNode.Tag), Helper.WindowManager.ControlPosition.Center);
                app.EventBus.Publish(new AreaSelectionEvent((Area)treeView.SelectedNode.Tag));
            }
        }

        private void MainForm_MapChanged(MapChangedEvent ev)
        {
            RebuildTree();
        }

        private void RebuildTree()
        {
            treeView.Nodes.Clear();
            TreeNode mapNode = new TreeNode(app.Map.Name);
            mapNode.ImageKey = "Map";
            mapNode.SelectedImageKey = "Map";
            mapNode.Tag = app.Map;
            foreach (var area in app.Map.Areas)
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
            if (app.Map == null)
                return;

            var a = new NewAreaForm();
            if (a.ShowDialog() == DialogResult.OK)
            {
                Area area = new Area(a.AreaName, app.Map.Areas.Count + 1, a.AreaWidth, a.AreaHeight);
                area.SpawnPoint = new Vector2(1, 1);

                var tiles = new Tile[area.Height * area.Width];

                for (int i = 0; i < tiles.Length; i++)
                    tiles[i] = new Tile(0, 0, false);

                Layer l = new Layer(0, tiles);

                area.Layers.Add(l);
                app.Map.Areas.Add(area);

                if (app.Map.StartArea == null)
                    app.Map.StartArea = area;

                app.EventBus.Publish(new AreaAddEvent(area));
            }

        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag is Map)
                app.Map.Name = e.Label;
        }
    }
}
