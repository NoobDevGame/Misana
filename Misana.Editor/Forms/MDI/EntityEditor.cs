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
    public partial class EntityEditor : SingleInstanceDockWindow, IMDIForm
    {
        public DockState DefaultDockState => DockState.Document;

        private MainForm mainForm;
        private EntityDefinition entityDefinition;

        public EntityEditor(MainForm mainForm, EntityDefinition entityDefinition)
        {
            InitializeComponent();

            textBox_name.Text = entityDefinition.Name;
            this.entityDefinition = entityDefinition;
            this.mainForm = mainForm;

            foreach(var cdef in entityDefinition.Definitions)
            {
                ListViewItem lvi = new ListViewItem(cdef.GetType().ToString());
                listView.Items.Add(lvi);
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            entityDefinition.Name = textBox_name.Text;
            mainForm.EventBus.Publish(new EntityDefinitionChangedEvent(entityDefinition));
        }

        private void EntityEditor_Load(object sender, EventArgs e)
        {

        }
    }
}
