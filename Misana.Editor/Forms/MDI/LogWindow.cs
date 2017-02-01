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

namespace Misana.Editor.Forms.MDI
{
    public partial class LogWindow : Control
    {
        private Application app;

        public LogWindow(Application mainForm) : base()
        {
            this.app = mainForm;

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

            listView.ContextMenuStrip = contextMenuStrip1;

            foreach (var ev in mainForm.Logger.Log)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.SubItems.Add(ev.Name);
                lvi.SubItems.Add(ev.Description);

                if (ev.GetType() == typeof(ErrorEvent))
                {
                    lvi.Text = "Error";
                    if (((ErrorEvent)ev).Exception != null)
                        lvi.SubItems.Add(((ErrorEvent)ev).Exception.Message);
                }

                listView.Items.Add(lvi);
            }
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearLog();
        }

        public void ClearLog()
        {
            listView.Items.Clear();
        }

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show();
            }
        }
    }
}
