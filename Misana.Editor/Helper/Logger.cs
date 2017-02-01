using Misana.Editor.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor.Helper
{
    public class Logger
    {
        private Application app;

        public List<ILoggerEvent> Log { get; private set; } = new List<ILoggerEvent>();

        public Logger(Application application)
        {
            application.EventBus.Subscribe<ErrorEvent>(HandleError);
        }

        private void HandleError(ErrorEvent ev)
        {
            if (ev.ShowDialog)
                MessageBox.Show(ev.Description, ev.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);

            Log.Add(ev);
        }
    }
}
