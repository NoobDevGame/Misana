using Misana.Core.Maps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor.Forms
{
    public partial class TiledMapConverterForm : Form
    {

        public string MapName { get; set; }

        public List<string> TiledAreas { get; set; } = new List<string>();

        public Map Map { get; private set; }

        public TiledMapConverterForm()
        {
            InitializeComponent();
        }

        private void button_convert_Click(object sender, EventArgs e)
        {
            MapName = textBox1.Text;
            Map  =  MapLoader.CreateMapFromTiled(MapName, TiledAreas.ToArray());
            Map.Name = MapName;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Tiled json (*.json)|*.json|All Files (*.*)|(*.*)";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                TiledAreas.Add(ofd.FileName);
                areaList.Items.Add(new ListViewItem(Path.GetFileName(ofd.FileName)) { Tag = ofd.FileName});
            }
        }

        private void button_remove_Click(object sender, EventArgs e)
        {
            if (areaList.SelectedItems.Count == 0)
                return;

            TiledAreas.Remove((string)areaList.SelectedItems[0].Tag);
            areaList.Items.RemoveByKey(areaList.SelectedItems[0].Name);
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
