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

namespace Misana.Editor.Forms.MDI
{
    public partial class EntityExplorer : Control
    {
        private Application app;

        public EntityExplorer(Application mainForm)
        {
            this.app = mainForm;

            InitializeComponent();

            mainForm.EventBus.Subscribe<AreaSelectionEvent>(t => AreaChanged(t.Area));

            mainForm.EventBus.Subscribe<EntityDefinitionChangedEvent>(EntityDefinitionChanged);

            mainForm.EventBus.Subscribe<MapChangedEvent>(t => LoadDefinitions());
        }

        private void LoadDefinitions()
        {
            if (app.Map == null)
                return;

            foreach(var edef in app.Map.GlobalEntityDefinitions)
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
                app.Map.GlobalEntityDefinitions.Remove(app.Map.GlobalEntityDefinitions.FirstOrDefault(t => t.Value == ev.EntityDefinition).Key);
                app.Map.GlobalEntityDefinitions.Add(ev.EntityDefinition.Name, ev.EntityDefinition);
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            if(app.Map == null)
            {
                app.EventBus.Publish(new ErrorEvent("Error", "No map opened"));
                return;
            }

            //EntityDefinition eDef = new EntityDefinition("Entity"+(listView.Items.Count+1));
            //app.Map.GlobalEntityDefinitions.Add(eDef.Name,eDef);
            //var lvi = new ListViewItem(eDef.Name) { Tag = eDef };
            //listView.Items.Add(lvi);
            

            //EntityEditor ee = new EntityEditor(app, eDef);
            ////app.WindowManager.AddShowWindow(ee);
        }

        public void RemoveEntityDefinition(EntityDefinition edef)
        {

        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            //    var window = app.WindowManager.Windows.FirstOrDefault(t => t.GetType() == typeof(EntityEditor) && ((EntityEditor)t).EntityDefinition == (EntityDefinition)listView.SelectedItems[0].Tag && t.Visible);

            //    if (window != null)
            //    {
            //        window.Select();
            //        window.Focus();
            //    }
            //    else
            //    {
            //        EntityEditor ee = new EntityEditor(app, (EntityDefinition)listView.SelectedItems[0].Tag);
            //        app.WindowManager.AddShowWindow(ee);
            //    }
        }

        private void listView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var x = (EntityDefinition) listView.SelectedItems[0].Tag;
            listView.DoDragDrop(x, DragDropEffects.Copy);
        }
    }
}
