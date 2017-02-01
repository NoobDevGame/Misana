using Misana.Core.Maps;
using Misana.Editor.Commands;
using Misana.Editor.Events;
using Misana.Editor.Forms.MDI;
using Misana.Editor.Helper;
using Misana.Editor.Models;
using Redbus.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor
{
    public partial class MainForm : Form
    {

        public TabControl TabLeftTop => tabControl_left_top;
        public TabControl TabLeftBottom => tabControl_left_bottom;
        public TabControl TabCenter => tabControl_center;
        public TabControl TabRightTop => tabControl_right_top;
        public TabControl TabRightBottom=> tabControl_right_bottom;

        private Application app;

        private MainFormCommands commandManager;

        public event EventHandler OnInitialize;

        public MainForm(Application application)
        {
            app = application;
            InitializeComponent();

            commandManager = new MainFormCommands(app);
        }

        protected override void OnLoad(EventArgs e)
        {
            TabCenter.MouseDown += TabCenter_MouseDown;
            OnInitialize?.Invoke(this,null);
        }

        private void TabCenter_MouseDown(object sender, MouseEventArgs e)
        {
           if(e.Button == MouseButtons.Middle)
            {
                if(TabCenter.GetTabRect(TabCenter.SelectedIndex).Contains(e.Location))
                {
                    app.WindowManager.CloseCenterTabPage(TabCenter.SelectedTab);
                }
            }
        }

        public void ShowErrorMessage(string message, string name = "Error")
        {
            MessageBox.Show(message, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region File Menu Commands
        private void menuItem_file_open_Click(object sender, EventArgs e) => commandManager.OpenMap();
        private void menuItem_file_save_Click(object sender, EventArgs e) => commandManager.SaveMap();
        private void menuItem_file_saveas_Click(object sender, EventArgs e) => commandManager.SaveMapAs();
        private void menuItem_file_exit_Click(object sender, EventArgs e) => commandManager.Exit();
        private void menuItem_file_import_tiledMap_Click(object sender, EventArgs e) => commandManager.ImportTiledMap();
        private void menuItem_file_import_tiled_Click(object sender, EventArgs e) => commandManager.ImportTiledMap();
        private void menuItem_file_new_Click(object sender, EventArgs e) => commandManager.CreateMap();
        #endregion
    }
}
