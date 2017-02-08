using Misana.Editor.Forms.MDI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor.Helper
{
    public sealed class WindowManager
    {
        private MainForm mainForm;

        private Dictionary<Control, ControlPane> controls = new Dictionary<Control, ControlPane>();

        private Application app;

        public LayerView LayerView { get; private set; }
        public MapView MapView { get; private set; }
        public PropertyView PropertyView { get; private set; }
        public EntityExplorer EntityExplorer { get; private set; }
        public EntityComponentToolbox EntityComponentToolbox { get; private set; }
        public TilesheetWindow TilesheetWindow { get; private set; }

        public WindowManager(Application app, MainForm mainForm)
        {
            this.mainForm = mainForm;
            this.app = app;
        }

        public void Initialize()
        {
            LayerView = new LayerView(app);
            AddControl(LayerView, ControlPosition.LeftBottom);

            MapView = new MapView(app);
            AddControl(MapView, ControlPosition.LeftTop);

            TilesheetWindow = new TilesheetWindow(app);
            AddControl(TilesheetWindow, ControlPosition.RightTop);

            PropertyView = new PropertyView(app);
            AddControl(PropertyView, ControlPosition.RightBottom);

            EntityComponentToolbox = new EntityComponentToolbox(app);
            AddControl(EntityComponentToolbox, ControlPosition.LeftTop);

            EntityExplorer = new EntityExplorer(app);
            AddControl(EntityExplorer, ControlPosition.RightTop);
        }

        public T GetWindow<T>() where T : Control
        {
            return (T)controls.FirstOrDefault(t => t.Key.GetType() == typeof(T)).Key;
        }

        public void AddControl(Control c, ControlPosition p, string name = null)
        {
            if (controls.ContainsKey(c))
                throw new NotSupportedException("Control already added");

            if (name == null)
                name = c.Text;

            TabPage page = new TabPage(name);
            c.Dock = DockStyle.Fill;
            page.Controls.Add(c);


            ControlPane cPane = new ControlPane(c, page, name, p);
            controls.Add(c, cPane);

            AddControlPane(cPane);
        }

        public void RemoveControl(Control c)
        {
            if (!controls.ContainsKey(c))
                throw new NotSupportedException("Control not added");

            ControlPane cPane = controls[c];
            RemoveControlPane(cPane);

            controls.Remove(c);
        }

        public void CloseCenterTabPage(TabPage tp)
        {
            var c = controls.FirstOrDefault(t => t.Value.TabPage == tp);
            RemoveControl(c.Key);
        }

        private void AddControlPane(ControlPane p)
        {
            TabControl tControl = GetTabControl(p.ControlPosition);
            tControl.TabPages.Add(p.TabPage);

            if (p.ControlPosition == ControlPosition.Center)
                tControl.SelectedTab = p.TabPage;
        }


        private void RemoveControlPane(ControlPane p)
        {
            TabControl tControl = GetTabControl(p.ControlPosition);
            tControl.TabPages.Remove(p.TabPage);
        }

        private TabControl GetTabControl(ControlPosition pos)
        {
            switch(pos)
            {
                case ControlPosition.Center:
                    return mainForm.TabCenter;
                case ControlPosition.LeftBottom:
                    return mainForm.TabLeftBottom;
                case ControlPosition.LeftTop:
                    return mainForm.TabLeftTop;
                case ControlPosition.RightBottom:
                    return mainForm.TabRightBottom;
                case ControlPosition.RightTop:
                    return mainForm.TabRightTop;
                default:
                    return null;
            }
        }

        private class ControlPane
        {
            public Control Control { get; set; }
            public TabPage TabPage { get; set; }
            public ControlPosition ControlPosition { get; set; }
            public string Name { get; set; }

            public ControlPane(Control c, TabPage t, string name, ControlPosition p)
            {
                Control = c;
                TabPage = t;
                ControlPosition = p;
                Name = name;
            }
        }

        public enum ControlPosition
        {
            Center,
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom
        }
    }
}
