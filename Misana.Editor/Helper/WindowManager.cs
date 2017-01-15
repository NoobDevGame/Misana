using Misana.Editor.Forms.MDI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace Misana.Editor.Helper
{
    public class WindowManager
    {
        public List<DockContent> Windows { get { return windows; } }

        private List<DockContent> windows = new List<DockContent>();

        private MainForm mainForm;
        private DockPanel dockPanel;

        public WindowManager(MainForm mainForm, DockPanel dockPanel)
        {
            this.mainForm = mainForm;
            this.dockPanel = dockPanel;
        }

        public void InitialLoad()
        {
            MapView mapView = new MapView(mainForm);
            AddWindow(mapView);

            TilesheetWindow ts = new TilesheetWindow(mainForm);
            AddWindow(ts);

            LayerView lv = new LayerView(mainForm);
            AddWindow(lv);

            PropertyView pv = new PropertyView(mainForm);
            AddWindow(pv);

            EntityExplorer ex = new EntityExplorer(mainForm);
            AddWindow(ex);

            EntityComponentToolbox ect = new EntityComponentToolbox(mainForm);
            AddWindow(ect);

            if (File.Exists("layout.xml"))
            {
                using (FileStream fs = File.Open("layout.xml", FileMode.Open))
                {
                    dockPanel.LoadFromXml(fs, (s) =>
                    {
                        if (s == typeof(MapView).ToString())
                            return mapView;
                        else if (s == typeof(TilesheetWindow).ToString())
                            return ts;
                        else if (s == typeof(LayerView).ToString())
                            return lv;
                        else if (s == typeof(PropertyView).ToString())
                            return pv;
                        else if (s == typeof(PropertyView).ToString())
                            return pv;
                        else if (s == typeof(EntityExplorer).ToString())
                            return ex;
                        else if (s == typeof(EntityComponentToolbox).ToString())
                            return ect;
                        return null;
                    });
                }
            }
            else
            {
                ShowWindow(mapView);
                ShowWindow(ts);
                ShowWindow(lv);
                ShowWindow(pv);
                ShowWindow(ex);
                ShowWindow(ect);
            }
        }

        public void SaveLayout()
        {
            dockPanel.SaveAsXml("layout.xml");
        }

        public void AddShowWindow(DockContent dockContent)
        {
            AddWindow(dockContent);
            ShowWindow(dockContent);
        }

        public void AddShowWindow(DockContent dockContent, DockState dockState)
        {
            AddWindow(dockContent);
            ShowWindow(dockContent, dockState);
        }

        public void AddWindow(DockContent dockContent)
        {
            Windows.Add(dockContent);
            dockContent.FormClosed += (s,e)=>
            {
                windows.Remove(dockContent);
                //TODO Raise Event
            };
        }

        public void ShowWindow(DockContent window)
        {
            window.Show(dockPanel, ((IMDIForm)window).DefaultDockState);
        }

        public void ShowWindow(DockContent window, DockState dockstate)
        {
            window.Show(dockPanel, dockstate);
        }

        public void ToggleWindow<T>() where T : DockContent
        {
            var window = GetWindow<T>();
            ToggleWindow<T>(window.IsHidden);
        }

        public void ToggleWindow<T>(bool show) where T : DockContent
        {
            if (typeof(T) != typeof(SingleInstanceDockWindow) && typeof(T).IsSubclassOf(typeof(SingleInstanceDockWindow)) == false)
                throw new NotSupportedException();

            var window = GetWindow<T>();

            if (window != null)
            {
                if (show && window.IsHidden)
                    window.Show();
                else if (!show && !window.IsHidden)
                    window.Hide();
            }
        }

        public T GetWindow<T>() where T : DockContent
        {
            return (T) Windows.FirstOrDefault(t => t.GetType() == typeof(T));
        }
    }
}
