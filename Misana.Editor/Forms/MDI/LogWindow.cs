using Misana.Editor.Events;
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
    public partial class LogWindow : DockContent
    {
        private MainForm mainForm;

        public LogWindow(MainForm mainForm)
        {
            this.mainForm = mainForm;

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach(var ev in mainForm.Logger.Log)
            {
                ListViewItem lvi = new ListViewItem();

                if (ev.GetType() == typeof(ErrorEvent))
                    lvi.Text = "Error";

                lvi.SubItems.Add(ev.Name);
                lvi.SubItems.Add(ev.Description);

                listView.Items.Add(lvi);
            }
        }
    }
}
