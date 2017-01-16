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

        public EntityDefinition EntityDefinition { get { return entityDefinition; } }

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

            mainForm.EventBus.Subscribe<EntityDefinitionChangedEvent>((ev) =>
            {
                if(ev.EntityDefinition == entityDefinition)
                {
                }
            });
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if(mainForm.Map.GlobalEntityDefinitions.ContainsKey(textBox_name.Text) && mainForm.Map.GlobalEntityDefinitions[textBox_name.Text] != entityDefinition)
            {
                mainForm.EventBus.Publish<ErrorEvent>(new ErrorEvent("Error", "Already a definition with that name"));
                return;
            }

            entityDefinition.Name = textBox_name.Text;
            mainForm.EventBus.Publish(new EntityDefinitionChangedEvent(entityDefinition));
        }

        private void EntityEditor_Load(object sender, EventArgs e)
        {

        }

        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void listView_DragDrop(object sender, DragEventArgs e)
        {
            var ecf = (Type) e.Data.GetData("System.RuntimeType");
            var def = (ComponentDefinition)Activator.CreateInstance(ecf);

            if (entityDefinition.Definitions.FirstOrDefault(t => t.GetType() == def.GetType()) != null)
                return;
            entityDefinition.Definitions.Add(def);
            listView.Items.Add(new ListViewItem(def.GetType().Name) { Tag = def });
            mainForm.EventBus.Publish(new EntityDefinitionChangedEvent(entityDefinition));
        }

        private void listView_Click(object sender, EventArgs e)
        {
            if(listView.SelectedItems.Count != null)
            {
                var def = entityDefinition.Definitions.FirstOrDefault(t => t.GetType() == listView.SelectedItems[0].Tag.GetType());

                if(def != null)
                    mainForm.WindowManager.GetWindow<PropertyView>().SelectObject(def);
            }
        }
    }
}
