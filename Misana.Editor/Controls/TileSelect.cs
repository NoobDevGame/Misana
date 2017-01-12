using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor.Controls
{
    public partial class TileSelect : UserControl
    {
        public Bitmap Texture { get; private set; }

        public TileSelect(Bitmap texture)
        {
            Texture = texture;

            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(Texture, new Point(0,0));

            base.OnPaint(e);

        }
    }
}
