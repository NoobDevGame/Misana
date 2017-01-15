namespace Misana.Editor.Forms.MDI
{
    partial class AreaRenderer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.control_panel = new System.Windows.Forms.Panel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.button_toggle_spawnpoint = new System.Windows.Forms.ToolStripButton();
            this.button_mode_select = new Misana.Editor.Controls.ToolStripRadioButton();
            this.button_mode_paint = new Misana.Editor.Controls.ToolStripRadioButton();
            this.button_mode_fill = new Misana.Editor.Controls.ToolStripRadioButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.button_toggle_spawnpoint,
            this.toolStripSeparator1,
            this.button_mode_select,
            this.button_mode_paint,
            this.button_mode_fill});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(284, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // control_panel
            // 
            this.control_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.control_panel.Location = new System.Drawing.Point(0, 25);
            this.control_panel.Name = "control_panel";
            this.control_panel.Size = new System.Drawing.Size(284, 236);
            this.control_panel.TabIndex = 1;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // button_toggle_spawnpoint
            // 
            this.button_toggle_spawnpoint.CheckOnClick = true;
            this.button_toggle_spawnpoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_toggle_spawnpoint.Image = global::Misana.Editor.Properties.Resources.IconCrosshair;
            this.button_toggle_spawnpoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_toggle_spawnpoint.Name = "button_toggle_spawnpoint";
            this.button_toggle_spawnpoint.Size = new System.Drawing.Size(23, 22);
            this.button_toggle_spawnpoint.Text = "toolStripButton1";
            this.button_toggle_spawnpoint.CheckStateChanged += new System.EventHandler(this.button_toggle_spawnpoint_CheckStateChanged);
            // 
            // button_mode_select
            // 
            this.button_mode_select.Checked = true;
            this.button_mode_select.CheckedColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(113)))), ((int)(((byte)(179)))));
            this.button_mode_select.CheckedColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(139)))), ((int)(((byte)(205)))));
            this.button_mode_select.CheckOnClick = true;
            this.button_mode_select.CheckState = System.Windows.Forms.CheckState.Checked;
            this.button_mode_select.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_mode_select.Image = global::Misana.Editor.Properties.Resources.IconCursorFilled;
            this.button_mode_select.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_mode_select.Name = "button_mode_select";
            this.button_mode_select.RadioButtonGroupId = 0;
            this.button_mode_select.Size = new System.Drawing.Size(23, 22);
            this.button_mode_select.Text = "Select";
            this.button_mode_select.CheckStateChanged += new System.EventHandler(this.ModeSelectionChanged);
            // 
            // button_mode_paint
            // 
            this.button_mode_paint.CheckedColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(113)))), ((int)(((byte)(179)))));
            this.button_mode_paint.CheckedColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(139)))), ((int)(((byte)(205)))));
            this.button_mode_paint.CheckOnClick = true;
            this.button_mode_paint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_mode_paint.Image = global::Misana.Editor.Properties.Resources.IconPen;
            this.button_mode_paint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_mode_paint.Name = "button_mode_paint";
            this.button_mode_paint.RadioButtonGroupId = 0;
            this.button_mode_paint.Size = new System.Drawing.Size(23, 22);
            this.button_mode_paint.Text = "Select";
            this.button_mode_paint.CheckStateChanged += new System.EventHandler(this.ModeSelectionChanged);
            // 
            // button_mode_fill
            // 
            this.button_mode_fill.CheckedColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(113)))), ((int)(((byte)(179)))));
            this.button_mode_fill.CheckedColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(139)))), ((int)(((byte)(205)))));
            this.button_mode_fill.CheckOnClick = true;
            this.button_mode_fill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_mode_fill.Image = global::Misana.Editor.Properties.Resources.IconFill;
            this.button_mode_fill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_mode_fill.Name = "button_mode_fill";
            this.button_mode_fill.RadioButtonGroupId = 0;
            this.button_mode_fill.Size = new System.Drawing.Size(23, 22);
            this.button_mode_fill.Text = "Select";
            this.button_mode_fill.CheckStateChanged += new System.EventHandler(this.ModeSelectionChanged);
            // 
            // AreaRenderer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.control_panel);
            this.Controls.Add(this.toolStrip);
            this.Name = "AreaRenderer";
            this.Text = "AreaRenderer";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.Panel control_panel;
        private System.Windows.Forms.ToolStripButton button_toggle_spawnpoint;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private Controls.ToolStripRadioButton button_mode_select;
        private Controls.ToolStripRadioButton button_mode_paint;
        private Controls.ToolStripRadioButton button_mode_fill;
    }
}