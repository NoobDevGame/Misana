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
    public partial class EntityExplorer : SingleInstanceDockWindow, IMDIForm
    {
        public DockState DefaultDockState => DockState.DockRight;

        private MainForm mainForm;

        public EntityExplorer(MainForm mainForm)
        {
            this.mainForm = mainForm;

            InitializeComponent();
        }
    }
}
