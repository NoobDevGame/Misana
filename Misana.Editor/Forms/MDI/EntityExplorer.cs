using Misana.Core.Entities;
using Misana.Core.Maps;
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

            mainForm.EventBus.Subscribe<AreaSelectionEvent>(t => AreaChanged(t.Area));

            mainForm.EventBus.Subscribe<EntityDefinitionChangedEvent>(EntityDefinitionChanged);

            mainForm.EventBus.Subscribe<MapChangedEvent>(t => LoadDefinitions());
        }

        private void LoadDefinitions()
        {
            if (mainForm.Map == null)
                return;

            foreach(var edef in mainForm.Map.GlobalEntityDefinitions)
            {
                listView.Items.Add(new ListViewItem(edef.Value.Name) { Tag = edef.Value });
            }
        }

        private void AreaChanged(Area a)
        {
            //listView.Items.Clear();

        }

        private void EntityDefinitionChanged(EntityDefinitionChangedEvent ev)
        {
            var item = listView.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Tag == ev.EntityDefinition);
            if (item != null)
            {
                item.Text = ev.EntityDefinition.Name;
                mainForm.Map.GlobalEntityDefinitions.Remove(mainForm.Map.GlobalEntityDefinitions.FirstOrDefault(t => t.Value == ev.EntityDefinition).Key);
                mainForm.Map.GlobalEntityDefinitions.Add(ev.EntityDefinition.Name, ev.EntityDefinition);
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            if(mainForm.Map == null)
            {
                mainForm.EventBus.Publish(new ErrorEvent("Error", "No map opened"));
                return;
            }

            EntityDefinition eDef = new EntityDefinition("Entity"+(listView.Items.Count+1));
            mainForm.Map.GlobalEntityDefinitions.Add(eDef.Name,eDef);
            var lvi = new ListViewItem(eDef.Name) { Tag = eDef };
            listView.Items.Add(lvi);
            

            EntityEditor ee = new EntityEditor(mainForm, eDef);
            mainForm.WindowManager.AddShowWindow(ee);
        }

        public void RemoveEntityDefinition(EntityDefinition edef)
        {

        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            var window = mainForm.WindowManager.Windows.FirstOrDefault(t => t.GetType() == typeof(EntityEditor) && ((EntityEditor)t).EntityDefinition == (EntityDefinition)listView.SelectedItems[0].Tag && t.Visible);

            if (window != null)
            {
                window.Select();
                window.Focus();
            }
            else
            {
                EntityEditor ee = new EntityEditor(mainForm, (EntityDefinition)listView.SelectedItems[0].Tag);
                mainForm.WindowManager.AddShowWindow(ee);
            }
        }

        private void listView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var x = (EntityDefinition) listView.SelectedItems[0].Tag;
            listView.DoDragDrop(x, DragDropEffects.Copy);
        }
    }
}
