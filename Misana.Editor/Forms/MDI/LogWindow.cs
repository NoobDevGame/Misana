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
    public partial class LogWindow : SingleInstanceDockWindow, IMDIForm
    {
        public DockState DefaultDockState => DockState.DockBottomAutoHide;

        private MainForm mainForm;

        public LogWindow(MainForm mainForm) : base()
        {
            this.mainForm = mainForm;

            InitializeComponent();

            mainForm.EventBus.Subscribe<ErrorEvent>((ev) =>
            {
                ListViewItem lvi = new ListViewItem();

                if (ev.GetType() == typeof(ErrorEvent))
                    lvi.Text = "Error";

                lvi.SubItems.Add(ev.Name);
                lvi.SubItems.Add(ev.Description);

                lvi.SubItems.Add(ev.Exception.Message);

                listView.Items.Add(lvi);
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach(var ev in mainForm.Logger.Log)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.SubItems.Add(ev.Name);
                lvi.SubItems.Add(ev.Description);

                if (ev.GetType() == typeof(ErrorEvent))
                {
                    lvi.Text = "Error";
                    lvi.SubItems.Add(((ErrorEvent)ev).Exception.Message);
                }

                listView.Items.Add(lvi);
            }
        }
    }
}
