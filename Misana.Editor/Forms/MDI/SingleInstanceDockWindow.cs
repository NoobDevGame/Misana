using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace Misana.Editor.Forms.MDI
{
    public class SingleInstanceDockWindow : DockContent
    {
        public SingleInstanceDockWindow()
        {
            HideOnClose = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            HideOnClose = true;
            base.OnLoad(e);
        }
    }
}
