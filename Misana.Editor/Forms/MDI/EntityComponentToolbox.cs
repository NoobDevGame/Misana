using Misana.Core.Entities;
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
    public partial class EntityComponentToolbox : Control
    {
        private Application app;

        public EntityComponentToolbox(Application mainForm)
        {
            this.app = mainForm;

            InitializeComponent();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var concreteTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract).ToList();

            var baseComponentType = typeof(ComponentDefinition);
            var componentTypes = concreteTypes
                .Where(t => baseComponentType.IsAssignableFrom(t))
                .ToList();

            foreach (var componentType in componentTypes)
            {
                ListViewItem lvi = new ListViewItem(componentType.Name);
                lvi.Tag = componentType;
                listView.Items.Add(lvi);
            }
        }

        private void listView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var x = (Type)listView.SelectedItems[0].Tag;
            listView.DoDragDrop(x, DragDropEffects.Copy);
        }
    }
}
