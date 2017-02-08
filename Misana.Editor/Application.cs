using Misana.Core.Maps;
using Misana.Editor.Events;
using Misana.Editor.Helper;
using Misana.Editor.Models;
using Redbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor
{
    public class Application
    {
        private MainForm mainForm;
        private WindowManager windowManager;

        public EventBus EventBus { get; private set; }
        public FileManager FileManager { get; private set; }
        public TilesheetManager TilesheetManager { get; private set; }
        public Logger Logger { get; private set; }
        public WindowManager WindowManager { get { return windowManager; } }

        public MapModel Map { get; private set; }

        public Application()
        {
            mainForm = new MainForm(this);
            windowManager = new WindowManager(this,mainForm);

            EventBus = new EventBus();
            FileManager = new FileManager(this);
            Logger = new Logger(this);
            TilesheetManager = new TilesheetManager(this);
            TilesheetManager.LoadTilesheets();

            mainForm.OnInitialize += (s, e) => OnInitialize();
        }

        public void Run()
        {
            System.Windows.Forms.Application.Run(mainForm);
        }

        public void OnInitialize()
        {
            windowManager.Initialize();
            SetMap(new MapModel("Unnamed"));
        }

        public void ShowErrorMessage(string message, string name = "Error")
        {
            mainForm.ShowErrorMessage(message, name);
        }

        public void SetMap(MapModel m)
        {
            Map = (m);
            EventBus.Publish(new MapChangedEvent(m));
        }

        internal void Close()
        {
            throw new NotImplementedException();
        }
    }
}
