using Misana.Core;
using Redbus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Editor.Events
{
    public class MapTileSelectionEvent : EventBase
    {
        public bool IsSingleTile {
            get
            {
                return (SelectionStart == SelectionEnd);
            }
        }

        public Index3 SelectionStart { get; set; }
        public Index3 SelectionEnd { get; set; }

        public MapTileSelectionEvent(Index3 selectionStart, Index3 selectionEnd)
        {
            SelectionStart = selectionStart;
            SelectionEnd = selectionEnd;
        }
    }
}
