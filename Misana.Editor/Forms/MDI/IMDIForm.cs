using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace Misana.Editor.Forms.MDI
{
    public interface IMDIForm
    {
        DockState DefaultDockState { get; }
    }
}
