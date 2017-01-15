using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Misana.Editor.Forms.Standalone
{
    public partial class NewAreaForm : Form
    {
        public string AreaName { get; set; }
        public int AreaHeight { get; set; }
        public int AreaWidth { get; set; }

        public NewAreaForm()
        {
            InitializeComponent();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (textBox_name.Text == "")
            {
                MessageBox.Show("Error", "Area name cannot be empty", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            AreaName = textBox_name.Text;
            AreaHeight = (int)numericUpDown_height.Value;
            AreaWidth = (int)numericUpDown_width.Value;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
