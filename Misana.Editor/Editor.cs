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

        public Dictionary<string,Image> TileSheets { get; private set; }

        internal RenderControl MapRenderer { get; private set; }

        public Editor()
        {
            TileSheets = new Dictionary<string, Image>();

            MapRenderer = new RenderControl(this);
            MapRenderer.Dock = DockStyle.Fill;


            InitializeComponent();


            rightSplit.Panel1.Controls.Add(MapRenderer);

            treeView_maps.ImageList = IconHelper.ImageList;

            MapRenderer.OnSelectionChanged += (s, e) =>
            {
                propertyGrid_tile.SelectedObject = TileClass.FromStruct(CurrentArea.Layers[0].Tiles[CurrentArea.GetTileIndex(MapRenderer.SelectedTile.X, MapRenderer.SelectedTile.Y)]);
            };

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            TileSheets.Add("TileSheetDungeon",Bitmap.FromFile("Content/TileSheetDungeon.png"));
            TileSheets.Add("TileSheetOutdoor",Bitmap.FromFile("Content/TileSheetOutdoor.png"));
            TileSheets.Add("TileSheetIndoor",Bitmap.FromFile("Content/TileSheetIndoor.png"));

            foreach(var TileSheet in TileSheets)
            {
                TabPage p = new TabPage(TileSheet.Key);
                p.Controls.Add(new TileSelect(new Bitmap(TileSheet.Value)) { Dock = DockStyle.Fill });
                tabControl_tileset.TabPages.Add(p);
                
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
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

                for (int y = 0; y < Map.Areas[i].Layers.Length; y++)
                {
                    areaNode.Nodes.Add(new TreeNode(Map.Areas[i].Layers[y].ToString()) { Tag = y, ImageKey = "Layer", SelectedImageKey = "Layer" });
                }

                rootNode.Nodes.Add(areaNode);

            }

            treeView_maps.Nodes.Clear();
            treeView_maps.Nodes.Add(rootNode);
            treeView_maps.ExpandAll();
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CreditsForm().ShowDialog();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
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

        private void ShowSaveDialog()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "mm";
            sfd.Filter = "Misana Map (*.mm)|*.mm";

            if (sfd.ShowDialog() == DialogResult.OK)
                savePath = sfd.FileName;
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSaveDialog();
            Save(true);
        }
    }
}
