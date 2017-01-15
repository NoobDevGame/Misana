using Misana.Core.Entities;
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
    public partial class EntityExplorer : SingleInstanceDockWindow, IMDIForm
    {
        public DockState DefaultDockState => DockState.DockRight;

        private MainForm mainForm;

        public EntityExplorer(MainForm mainForm)
        {
            this.mainForm = mainForm;

            InitializeComponent();

            mainForm.EventBus.Subscribe<EntityDefinitionChangedEvent>(EntityDefinitionChanged);
        }

        private void EntityDefinitionChanged(EntityDefinitionChangedEvent ev)
        {
            var item = listView.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Tag == ev.EntityDefinition);
            if (item != null)
                item.Text = ev.EntityDefinition.Name;
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            EntityDefinition eDef = new EntityDefinition("Entity"+listView.Items.Count+1);
            listView.Items.Add(new ListViewItem(eDef.Name) { Tag = eDef });
            

            EntityEditor ee = new EntityEditor(mainForm, eDef);
            mainForm.WindowManager.AddShowWindow(ee);
        }
    }
}
