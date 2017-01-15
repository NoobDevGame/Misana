using Misana.Core.Maps;
using Misana.Editor.Commands;
using Misana.Editor.Events;
using Misana.Editor.Forms.MDI;
using Misana.Editor.Helper;
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
using WeifenLuo.WinFormsUI.Docking;

namespace Misana.Editor
{
    public partial class MainForm : Form
    {
        public Redbus.EventBus EventBus { get; private set; }

        public Map Map { get; private set; }

        public FileManager FileManager { get; private set; }

        public TilesheetManager TilesheetManager { get; private set; }

        private MainFormCommands commandManager;

        public Logger Logger { get; private set; }

        public WindowManager WindowManager { get; private set; }

        public VisualStudioToolStripExtender VSToolStripExtender { get { return visualStudioToolStripExtender1; } }

        public ThemeBase Theme { get { return vS2015LightTheme1; } }

        public MainForm()
        {
            InitializeComponent();

            commandManager = new MainFormCommands(this);

            this.EventBus = new Redbus.EventBus();

            FileManager = new FileManager(this);

            TilesheetManager = new TilesheetManager(this);

            Logger = new Logger(this);

            WindowManager = new WindowManager(this, dockPanel);

            this.dockPanel.Theme = Theme;
            visualStudioToolStripExtender1.SetStyle(statusStrip, VisualStudioToolStripExtender.VsVersion.Vs2015, Theme);
            visualStudioToolStripExtender1.SetStyle(menuStrip, VisualStudioToolStripExtender.VsVersion.Vs2015, Theme);

        }

        protected override void OnLoad(EventArgs e)
        {
            TilesheetManager.LoadTilesheets();

            EventBus.Subscribe<AreaSelectionEvent>((a) =>
            {
                AreaRenderer ar = new AreaRenderer(this, a.Area);
                WindowManager.AddWindow(ar);
                ar.Show(dockPanel, DockState.Document);
            }); 

            WindowManager.InitialLoad();
            base.OnLoad(e);
        }

        public void SetMap(Map map)
        {
            Map = map;
            EventBus.Publish(new MapChangedEvent(map));
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
        #endregion

        #region View Commands
        private void menuItem_view_log_CheckStateChanged(object sender, EventArgs e) => commandManager.LogStateChanged(menuItem_view_log.Checked);

        private void menuItem_view_map_CheckedChanged(object sender, EventArgs e) => WindowManager.ToggleWindow<MapView>(menuItem_view_map.Checked);

        private void menuItem_view_tilesheets_CheckedChanged(object sender, EventArgs e) => WindowManager.ToggleWindow<TilesheetWindow>(menuItem_view_tilesheets.Checked);

        private void menuItem_view_layerView_CheckedChanged(object sender, EventArgs e) => WindowManager.ToggleWindow<LayerView>(menuItem_view_layerView.Checked);

        private void menuItem_view_properties_CheckedChanged(object sender, EventArgs e) => Console.WriteLine("TEMP");

        #endregion

        private void saveLayoutToolStripMenuItem_Click(object sender, EventArgs e) => WindowManager.SaveLayout();

        private void menu_view_entityexplorer_CheckedChanged(object sender, EventArgs e) => WindowManager.ToggleWindow<EntityExplorer>();

        private void menuItem_file_import_tiled_Click(object sender, EventArgs e) => commandManager.ImportTiledMap();
    }
}
