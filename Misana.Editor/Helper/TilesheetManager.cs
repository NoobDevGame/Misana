using Misana.Core.Maps;
using Misana.Editor.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Helper
{
    public class TilesheetManager
    {
        public Dictionary<string, Tilesheet> Tilesheets { get { return tilesheets; } }

        private Application app;
        private Dictionary<string,Tilesheet> tilesheets = new Dictionary<string, Tilesheet>();

        public TilesheetManager(Application application)
        {
            this.app = application;
        }

        public void LoadTilesheets(string path = "Content/Tilesheets/")
        {
            foreach (var tf in Directory.GetFiles(path, "*.json"))
            {
                LoadTilesheet(tf);
            }
        }

        public void LoadTilesheet(string path)
        {
            try
            {
                var ts = Tilesheet.LoadTilesheet(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                tilesheets.Add(ts.Name, ts);
            }
            catch (Exception e)
            {
                app.EventBus.Publish(new ErrorEvent("Tilesheet", "Could not load tilesheet " + Path.GetFileNameWithoutExtension(path)));
            }
        }
    }
}
