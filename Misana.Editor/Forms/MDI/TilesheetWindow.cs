using Misana.Core;
using Misana.Editor.Controls;
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
    public partial class TilesheetWindow : SingleInstanceDockWindow, IMDIForm
    {
        public DockState DefaultDockState => DockState.DockRight;

        private MainForm mainForm;

        public string SelectedTilesheetName { get { return tileSelect.Selection.Key; } }

        public int SelectedTileID { get { return tileSelect.SelectedTileId; } }

        public Index2 SelectedTileIndex { get { return tileSelect.Selection.Value; } }

        private TileSelect tileSelect;

        public TilesheetWindow(MainForm mainForm) : base()
        {
            this.mainForm = mainForm;

            InitializeComponent();

            tileSelect = new TileSelect(mainForm);
            tileSelect.Dock = DockStyle.Fill;
            Controls.Add(tileSelect);
        }
    }
}
