using Misana.Core.Maps;
using Misana.Editor.Forms.MDI;
using Misana.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Commands
{
    internal class MainFormCommands
    {
        private Application application;

        public MainFormCommands(Application application)
        {
            this.application = application;
        }

        #region File Menu

        public void CreateMap()
        {
            var map = new MapModel("Map");
            application.SetMap(map);
        }

        public void OpenMap()
        {
            var map = application.FileManager.OpenMap();
            if (map != null)
                application.SetMap(new MapModel(map));
        }

        public void ImportTiledMap()
        {
            var map = application.FileManager.ImportTiledMap();
            if (map != null)
                application.SetMap(new MapModel(map));
        }

        public void SaveMap()
        {
            application.FileManager.SaveMap();
        }

        public void SaveMapAs()
        {
            application.FileManager.SaveMapAs();
        }

        public void Exit()
        {
            application.Close();
        }

        #endregion

        #region Map Menu

        #endregion

        #region View Menu
        internal void LogStateChanged(bool showLog)
        {
           
        }
        #endregion
    }
}
