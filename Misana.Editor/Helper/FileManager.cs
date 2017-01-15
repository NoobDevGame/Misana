using Misana.Core.Maps;
using Misana.Editor.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor.Helper
{
    public class FileManager
    {
        public string SavePath {get;set;}

        private string originalPath;

        private MainForm mainForm;

        public FileManager(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        //public Map NewMap()
        //{

        //}

        public Map OpenMap()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.AddExtension = true;
            openFileDialog.AutoUpgradeEnabled = true;
            //openFileDialog.DefaultExt = "mm";
            openFileDialog.Filter = "Misana Maps (*.mm)|*.mm|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalPath = openFileDialog.FileName;
                SavePath = originalPath;
                var map = MapLoader.LoadPath(openFileDialog.FileName);
                return map;
            }
            else
            {
                return null;
            }
        }

        public Map ImportTiledMap()
        {
            TiledMapConverterForm tmcf = new TiledMapConverterForm();
            if (tmcf.ShowDialog() == DialogResult.OK)
            {
                var map = tmcf.Map;
                originalPath = "";
                SavePath = "";
                return map;
            }
            else
                return null;
        }

        public void SaveMap()
        {
            if(SavePath == null || SavePath == "")
                SaveMapAs();
            else
                InternalSaveMap();
        }

        public void SaveMapAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "mm";
            sfd.Filter = "Misana Map (*.mm)|*.mm";

            if (sfd.ShowDialog() == DialogResult.OK)
                SavePath = sfd.FileName;

            InternalSaveMap();
        }

        private void InternalSaveMap()
        {
            if (SavePath == null || SavePath == "")
                return;

            try
            {
                MapLoader.Save(mainForm.Map, SavePath);
            }
            catch(Exception ex)
            {
                mainForm.ShowErrorMessage("Could not save map!", "Save Error");
            }
        }
    }
}
