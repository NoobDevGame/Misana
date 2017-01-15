using Redbus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Events
{
    public class ErrorEvent : EventBase, ILoggerEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShowDialog { get; set; }
        public Exception Exception { get; set; }

        public ErrorEvent(string name, string description, bool showDialog = true, Exception exception = null)
        {
            Name = name;
            Description = description;
            ShowDialog = showDialog;
            Exception = exception;
        }
    }
}
