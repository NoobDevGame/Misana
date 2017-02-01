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

namespace Misana.Editor.Forms.MDI
{
    public partial class TilesheetWindow : Control
    {
        private Application app;

        public string SelectedTilesheetName { get { return tileSelect.Selection.Key; } }

        public int SelectedTileID { get { return tileSelect.SelectedTileId; } }

        public Index2 SelectedTileIndex { get { return tileSelect.Selection.Value; } }

        private TileSelect tileSelect;

        public TilesheetWindow(Application mainForm) : base()
        {
            this.app = mainForm;

            InitializeComponent();

            tileSelect = new TileSelect(mainForm);
            tileSelect.Dock = DockStyle.Fill;
            Controls.Add(tileSelect);
        }
    }
}
