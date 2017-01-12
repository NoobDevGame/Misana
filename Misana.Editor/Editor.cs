using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Misana.Core.Maps;
using Misana.Core.Maps.MapSerializers;
using Misana.Editor.Forms;
using Misana.Editor.Controls;
using System.IO;

namespace Misana.Editor
{
    public partial class Editor : Form
    {
        public Map Map { get { return map; } private set
            {
                if (map == value)
                    return;

                map = value;
                RecalculateTree();
                CurrentArea = map.Areas[0];
            }
        }
        private Map map;

        public Area CurrentArea { get { return currenArea; }
            private set {
                if (currenArea == value)
                    return;
                else
                {
                    currenArea = value;
                    MapRenderer?.Invalidate();
                }
            }
        }

        public string savePath;

        private Area currenArea;

        public Dictionary<string,Tilesheet> TileSheets { get; private set; }

        internal RenderControl MapRenderer { get; private set; }

        public Editor()
        {
            TileSheets = new Dictionary<string, Tilesheet>();

            MapRenderer = new RenderControl(this);
            MapRenderer.Dock = DockStyle.Fill;


            InitializeComponent();


            rightSplit.Panel1.Controls.Add(MapRenderer);

            treeView_maps.ImageList = IconHelper.ImageList;

            MapRenderer.OnSelectionChanged += (s, e) =>
            {
                //propertyGrid_tile.SelectedObject = TileClass.FromStruct(CurrentArea.Layers[0].Tiles[CurrentArea.GetTileIndex(MapRenderer.SelectedTile.X, MapRenderer.SelectedTile.Y)]);
            };

        }

        private void LoadTilesheets() //TODO auslagern
        {
            foreach (var tf in Directory.GetFiles("Content/Tilesheets/", "*.json"))
            {
                try
                {
                    var ts = Tilesheet.LoadTilesheet("Content/Tilesheets/", Path.GetFileNameWithoutExtension(tf));
                    TileSheets.Add(ts.Name, ts);
                }catch(Exception e)
                {
                    MessageBox.Show("Could not load tilesheet " + Path.GetFileNameWithoutExtension(tf), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            foreach(var tilesheet in TileSheets)
            {
                TabPage tp = new TabPage(tilesheet.Value.Name);

                TileSelect ts = new TileSelect(tilesheet.Value.Texture);
                ts.Dock = DockStyle.Fill;

                tp.Controls.Add(ts);

                tabControl_tileset.TabPages.Add(tp);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            LoadTilesheets();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.AddExtension = true;
            openFileDialog.AutoUpgradeEnabled = true;
            //openFileDialog.DefaultExt = "mm";
            openFileDialog.Filter = "Misana Maps (*.mm)|*.mm|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Map = MapLoader.LoadPath(openFileDialog.FileName);
            }
            
        }

        private void RecalculateTree()
        {
            TreeNode rootNode = new TreeNode(Map.Name);
            rootNode.ImageKey = "Map";
            rootNode.SelectedImageKey = "Map";

            for (int i = 0; i < Map.Areas.Length; i++)
            {
                var areaNode = new TreeNode(Map.Areas[i].Name) { Tag = i, ImageKey = "Area", SelectedImageKey = "Area" };
                rootNode.Nodes.Add(areaNode);
            }

            treeView_maps.Nodes.Clear();
            treeView_maps.Nodes.Add(rootNode);
            treeView_maps.ExpandAll();
        }

        private void treeView_maps_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageKey == "Map")
                return;

            if (e.Node.ImageKey == "Area")
                CurrentArea = Map.Areas[(int)e.Node.Tag];
        }

        private void convertTiledMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TiledMapConverterForm tmcf = new TiledMapConverterForm();
            if(tmcf.ShowDialog() == DialogResult.OK)
            {
                Map = tmcf.Map;
            }
        }

        private void ShowSaveDialog(bool save = false)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "mm";
            sfd.Filter = "Misana Map (*.mm)|*.mm";

            if (sfd.ShowDialog() == DialogResult.OK)
                savePath = sfd.FileName;

            if (save)
                Save(true);
        }

        private void Save(bool diaShown = false)
        {
            if (savePath == null || savePath == "")
            {
                if (diaShown)
                    return;
                else
                    ShowSaveDialog();
            }

            try
            {
                MapLoader.Save(Map,savePath);
            }catch(Exception e)
            {
                MessageBox.Show("Map could not be saved!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => this.Close();

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e) => new CreditsForm().ShowDialog();

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e) => new About().ShowDialog();

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) => Save();

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) => ShowSaveDialog(true);
    }
}
