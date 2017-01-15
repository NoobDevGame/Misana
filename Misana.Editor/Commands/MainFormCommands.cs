using Misana.Editor.Forms.MDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Commands
{
    internal class MainFormCommands
    {
        private MainForm mainForm;

        public MainFormCommands(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        #region File Menu

        public void OpenMap()
        {
            var map = mainForm.FileManager.OpenMap();
            if (map != null)
                mainForm.SetMap(map);
        }

        public void ImportTiledMap()
        {
            var map = mainForm.FileManager.ImportTiledMap();
            if (map != null)
                mainForm.SetMap(map);
        }

        public void SaveMap()
        {
            mainForm.FileManager.SaveMap();
        }

        public void SaveMapAs()
        {
            mainForm.FileManager.SaveMapAs();
        }

        public void Exit()
        {
            mainForm.Close();
        }

        #endregion

        #region Map Menu

        #endregion

        #region View Menu
        internal void LogStateChanged(bool showLog)
        {
            if(showLog)
            {
                if (mainForm.WindowManager.Windows.FirstOrDefault(t => t.GetType() == typeof(LogWindow)) != null)
                    return;

                LogWindow lWindow = new LogWindow(mainForm);
                mainForm.WindowManager.AddWindow(lWindow);
                mainForm.WindowManager.ShowWindow(lWindow, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);

            }
            else
            {
                var windows = mainForm.WindowManager.Windows.Where(t => t.GetType() == typeof(LogWindow)).ToArray();
                for(int i = 0; i < windows.Count(); i++)
                    windows[i].Close();
            }
        }
        #endregion
    }
}
