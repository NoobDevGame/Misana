using Misana.Editor.Forms.MDI;
using System;
using System.Collections.Generic;
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
            mapView.Show(dockPanel, DockState.DockLeft);

            TilesheetWindow ts = new TilesheetWindow(mainForm);
            AddWindow(ts);
            ShowWindow(ts,DockState.DockRight);
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

        public void ShowWindow(DockContent window, DockState dockstate)
        {
            window.Show(dockPanel, dockstate);
        }

        public T GetWindow<T>() where T : DockContent
        {
            return (T) Windows.FirstOrDefault(t => t.GetType() == typeof(T));
        }
    }
}
