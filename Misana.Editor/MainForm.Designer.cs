namespace Misana.Editor
{
    partial class MainForm
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuItem_file = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_file_new = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_file_open = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_file_import = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_file_import_tiled = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_file_save = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_file_saveas = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_file_divider1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem_file_exit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_map = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_map_properties = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem_map_addArea = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_map_importArea = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_map_importArea_tiled = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_about = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_about_about = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_about_credits = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_about_help = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer_left = new System.Windows.Forms.SplitContainer();
            this.splitContainer_left_v = new System.Windows.Forms.SplitContainer();
            this.tabControl_left_top = new System.Windows.Forms.TabControl();
            this.tabControl_left_bottom = new System.Windows.Forms.TabControl();
            this.splitContainer_right = new System.Windows.Forms.SplitContainer();
            this.tabControl_center = new System.Windows.Forms.TabControl();
            this.splitContainer_right_v = new System.Windows.Forms.SplitContainer();
            this.tabControl_right_top = new System.Windows.Forms.TabControl();
            this.tabControl_right_bottom = new System.Windows.Forms.TabControl();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_left)).BeginInit();
            this.splitContainer_left.Panel1.SuspendLayout();
            this.splitContainer_left.Panel2.SuspendLayout();
            this.splitContainer_left.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_left_v)).BeginInit();
            this.splitContainer_left_v.Panel1.SuspendLayout();
            this.splitContainer_left_v.Panel2.SuspendLayout();
            this.splitContainer_left_v.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_right)).BeginInit();
            this.splitContainer_right.Panel1.SuspendLayout();
            this.splitContainer_right.Panel2.SuspendLayout();
            this.splitContainer_right.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_right_v)).BeginInit();
            this.splitContainer_right_v.Panel1.SuspendLayout();
            this.splitContainer_right_v.Panel2.SuspendLayout();
            this.splitContainer_right_v.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 611);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1249, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_file,
            this.menuItem_map,
            this.menuItem_about});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1249, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // menuItem_file
            // 
            this.menuItem_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_file_new,
            this.menuItem_file_open,
            this.menuItem_file_import,
            this.menuItem_file_save,
            this.menuItem_file_saveas,
            this.menuItem_file_divider1,
            this.menuItem_file_exit});
            this.menuItem_file.Name = "menuItem_file";
            this.menuItem_file.Size = new System.Drawing.Size(37, 20);
            this.menuItem_file.Text = "File";
            // 
            // menuItem_file_new
            // 
            this.menuItem_file_new.Name = "menuItem_file_new";
            this.menuItem_file_new.Size = new System.Drawing.Size(112, 22);
            this.menuItem_file_new.Text = "New";
            this.menuItem_file_new.Click += new System.EventHandler(this.menuItem_file_new_Click);
            // 
            // menuItem_file_open
            // 
            this.menuItem_file_open.Name = "menuItem_file_open";
            this.menuItem_file_open.Size = new System.Drawing.Size(112, 22);
            this.menuItem_file_open.Text = "Open";
            this.menuItem_file_open.Click += new System.EventHandler(this.menuItem_file_open_Click);
            // 
            // menuItem_file_import
            // 
            this.menuItem_file_import.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_file_import_tiled});
            this.menuItem_file_import.Name = "menuItem_file_import";
            this.menuItem_file_import.Size = new System.Drawing.Size(112, 22);
            this.menuItem_file_import.Text = "Import";
            // 
            // menuItem_file_import_tiled
            // 
            this.menuItem_file_import_tiled.Name = "menuItem_file_import_tiled";
            this.menuItem_file_import_tiled.Size = new System.Drawing.Size(127, 22);
            this.menuItem_file_import_tiled.Text = "Tiled map";
            this.menuItem_file_import_tiled.Click += new System.EventHandler(this.menuItem_file_import_tiled_Click);
            // 
            // menuItem_file_save
            // 
            this.menuItem_file_save.Name = "menuItem_file_save";
            this.menuItem_file_save.Size = new System.Drawing.Size(112, 22);
            this.menuItem_file_save.Text = "Save";
            this.menuItem_file_save.Click += new System.EventHandler(this.menuItem_file_save_Click);
            // 
            // menuItem_file_saveas
            // 
            this.menuItem_file_saveas.Name = "menuItem_file_saveas";
            this.menuItem_file_saveas.Size = new System.Drawing.Size(112, 22);
            this.menuItem_file_saveas.Text = "Save as";
            this.menuItem_file_saveas.Click += new System.EventHandler(this.menuItem_file_saveas_Click);
            // 
            // menuItem_file_divider1
            // 
            this.menuItem_file_divider1.Name = "menuItem_file_divider1";
            this.menuItem_file_divider1.Size = new System.Drawing.Size(109, 6);
            // 
            // menuItem_file_exit
            // 
            this.menuItem_file_exit.Name = "menuItem_file_exit";
            this.menuItem_file_exit.Size = new System.Drawing.Size(112, 22);
            this.menuItem_file_exit.Text = "Exit";
            this.menuItem_file_exit.Click += new System.EventHandler(this.menuItem_file_exit_Click);
            // 
            // menuItem_map
            // 
            this.menuItem_map.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_map_properties,
            this.toolStripMenuItem1,
            this.menuItem_map_addArea,
            this.menuItem_map_importArea});
            this.menuItem_map.Name = "menuItem_map";
            this.menuItem_map.Size = new System.Drawing.Size(43, 20);
            this.menuItem_map.Text = "Map";
            // 
            // menuItem_map_properties
            // 
            this.menuItem_map_properties.Name = "menuItem_map_properties";
            this.menuItem_map_properties.Size = new System.Drawing.Size(137, 22);
            this.menuItem_map_properties.Text = "Properties";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(134, 6);
            // 
            // menuItem_map_addArea
            // 
            this.menuItem_map_addArea.Name = "menuItem_map_addArea";
            this.menuItem_map_addArea.Size = new System.Drawing.Size(137, 22);
            this.menuItem_map_addArea.Text = "Add Area";
            // 
            // menuItem_map_importArea
            // 
            this.menuItem_map_importArea.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_map_importArea_tiled});
            this.menuItem_map_importArea.Name = "menuItem_map_importArea";
            this.menuItem_map_importArea.Size = new System.Drawing.Size(137, 22);
            this.menuItem_map_importArea.Text = "Import Area";
            // 
            // menuItem_map_importArea_tiled
            // 
            this.menuItem_map_importArea_tiled.Name = "menuItem_map_importArea_tiled";
            this.menuItem_map_importArea_tiled.Size = new System.Drawing.Size(127, 22);
            this.menuItem_map_importArea_tiled.Text = "Tiled Area";
            // 
            // menuItem_about
            // 
            this.menuItem_about.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_about_about,
            this.menuItem_about_credits,
            this.menuItem_about_help});
            this.menuItem_about.Name = "menuItem_about";
            this.menuItem_about.Size = new System.Drawing.Size(52, 20);
            this.menuItem_about.Text = "About";
            // 
            // menuItem_about_about
            // 
            this.menuItem_about_about.Name = "menuItem_about_about";
            this.menuItem_about_about.Size = new System.Drawing.Size(111, 22);
            this.menuItem_about_about.Text = "About";
            // 
            // menuItem_about_credits
            // 
            this.menuItem_about_credits.Name = "menuItem_about_credits";
            this.menuItem_about_credits.Size = new System.Drawing.Size(111, 22);
            this.menuItem_about_credits.Text = "Credits";
            // 
            // menuItem_about_help
            // 
            this.menuItem_about_help.Name = "menuItem_about_help";
            this.menuItem_about_help.Size = new System.Drawing.Size(111, 22);
            this.menuItem_about_help.Text = "Help";
            // 
            // splitContainer_left
            // 
            this.splitContainer_left.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_left.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer_left.Location = new System.Drawing.Point(0, 24);
            this.splitContainer_left.Name = "splitContainer_left";
            // 
            // splitContainer_left.Panel1
            // 
            this.splitContainer_left.Panel1.Controls.Add(this.splitContainer_left_v);
            // 
            // splitContainer_left.Panel2
            // 
            this.splitContainer_left.Panel2.Controls.Add(this.splitContainer_right);
            this.splitContainer_left.Size = new System.Drawing.Size(1249, 587);
            this.splitContainer_left.SplitterDistance = 249;
            this.splitContainer_left.TabIndex = 5;
            // 
            // splitContainer_left_v
            // 
            this.splitContainer_left_v.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_left_v.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_left_v.Name = "splitContainer_left_v";
            this.splitContainer_left_v.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_left_v.Panel1
            // 
            this.splitContainer_left_v.Panel1.Controls.Add(this.tabControl_left_top);
            // 
            // splitContainer_left_v.Panel2
            // 
            this.splitContainer_left_v.Panel2.Controls.Add(this.tabControl_left_bottom);
            this.splitContainer_left_v.Size = new System.Drawing.Size(249, 587);
            this.splitContainer_left_v.SplitterDistance = 248;
            this.splitContainer_left_v.TabIndex = 0;
            // 
            // tabControl_left_top
            // 
            this.tabControl_left_top.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_left_top.Location = new System.Drawing.Point(0, 0);
            this.tabControl_left_top.Name = "tabControl_left_top";
            this.tabControl_left_top.SelectedIndex = 0;
            this.tabControl_left_top.Size = new System.Drawing.Size(249, 248);
            this.tabControl_left_top.TabIndex = 0;
            // 
            // tabControl_left_bottom
            // 
            this.tabControl_left_bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_left_bottom.Location = new System.Drawing.Point(0, 0);
            this.tabControl_left_bottom.Name = "tabControl_left_bottom";
            this.tabControl_left_bottom.SelectedIndex = 0;
            this.tabControl_left_bottom.Size = new System.Drawing.Size(249, 335);
            this.tabControl_left_bottom.TabIndex = 1;
            // 
            // splitContainer_right
            // 
            this.splitContainer_right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_right.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer_right.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_right.Name = "splitContainer_right";
            // 
            // splitContainer_right.Panel1
            // 
            this.splitContainer_right.Panel1.Controls.Add(this.tabControl_center);
            // 
            // splitContainer_right.Panel2
            // 
            this.splitContainer_right.Panel2.Controls.Add(this.splitContainer_right_v);
            this.splitContainer_right.Size = new System.Drawing.Size(996, 587);
            this.splitContainer_right.SplitterDistance = 672;
            this.splitContainer_right.TabIndex = 0;
            // 
            // tabControl_center
            // 
            this.tabControl_center.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_center.Location = new System.Drawing.Point(0, 0);
            this.tabControl_center.Name = "tabControl_center";
            this.tabControl_center.SelectedIndex = 0;
            this.tabControl_center.Size = new System.Drawing.Size(672, 587);
            this.tabControl_center.TabIndex = 2;
            // 
            // splitContainer_right_v
            // 
            this.splitContainer_right_v.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_right_v.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_right_v.Name = "splitContainer_right_v";
            this.splitContainer_right_v.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_right_v.Panel1
            // 
            this.splitContainer_right_v.Panel1.Controls.Add(this.tabControl_right_top);
            // 
            // splitContainer_right_v.Panel2
            // 
            this.splitContainer_right_v.Panel2.Controls.Add(this.tabControl_right_bottom);
            this.splitContainer_right_v.Size = new System.Drawing.Size(320, 587);
            this.splitContainer_right_v.SplitterDistance = 256;
            this.splitContainer_right_v.TabIndex = 0;
            // 
            // tabControl_right_top
            // 
            this.tabControl_right_top.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_right_top.Location = new System.Drawing.Point(0, 0);
            this.tabControl_right_top.Name = "tabControl_right_top";
            this.tabControl_right_top.SelectedIndex = 0;
            this.tabControl_right_top.Size = new System.Drawing.Size(320, 256);
            this.tabControl_right_top.TabIndex = 2;
            // 
            // tabControl_right_bottom
            // 
            this.tabControl_right_bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_right_bottom.Location = new System.Drawing.Point(0, 0);
            this.tabControl_right_bottom.Name = "tabControl_right_bottom";
            this.tabControl_right_bottom.SelectedIndex = 0;
            this.tabControl_right_bottom.Size = new System.Drawing.Size(320, 327);
            this.tabControl_right_bottom.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 633);
            this.Controls.Add(this.splitContainer_left);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer_left.Panel1.ResumeLayout(false);
            this.splitContainer_left.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_left)).EndInit();
            this.splitContainer_left.ResumeLayout(false);
            this.splitContainer_left_v.Panel1.ResumeLayout(false);
            this.splitContainer_left_v.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_left_v)).EndInit();
            this.splitContainer_left_v.ResumeLayout(false);
            this.splitContainer_right.Panel1.ResumeLayout(false);
            this.splitContainer_right.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_right)).EndInit();
            this.splitContainer_right.ResumeLayout(false);
            this.splitContainer_right_v.Panel1.ResumeLayout(false);
            this.splitContainer_right_v.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_right_v)).EndInit();
            this.splitContainer_right_v.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItem_file;
        private System.Windows.Forms.ToolStripMenuItem menuItem_map;
        private System.Windows.Forms.ToolStripMenuItem menuItem_about;
        private System.Windows.Forms.ToolStripMenuItem menuItem_file_new;
        private System.Windows.Forms.ToolStripMenuItem menuItem_file_open;
        private System.Windows.Forms.ToolStripMenuItem menuItem_file_import;
        private System.Windows.Forms.ToolStripMenuItem menuItem_file_save;
        private System.Windows.Forms.ToolStripMenuItem menuItem_file_saveas;
        private System.Windows.Forms.ToolStripSeparator menuItem_file_divider1;
        private System.Windows.Forms.ToolStripMenuItem menuItem_file_exit;
        private System.Windows.Forms.ToolStripMenuItem menuItem_map_properties;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuItem_map_addArea;
        private System.Windows.Forms.ToolStripMenuItem menuItem_map_importArea;
        private System.Windows.Forms.ToolStripMenuItem menuItem_map_importArea_tiled;
        private System.Windows.Forms.ToolStripMenuItem menuItem_about_about;
        private System.Windows.Forms.ToolStripMenuItem menuItem_about_credits;
        private System.Windows.Forms.ToolStripMenuItem menuItem_about_help;
        private System.Windows.Forms.ToolStripMenuItem menuItem_file_import_tiled;
        private System.Windows.Forms.SplitContainer splitContainer_left;
        private System.Windows.Forms.SplitContainer splitContainer_left_v;
        private System.Windows.Forms.TabControl tabControl_left_top;
        private System.Windows.Forms.TabControl tabControl_left_bottom;
        private System.Windows.Forms.SplitContainer splitContainer_right;
        private System.Windows.Forms.TabControl tabControl_center;
        private System.Windows.Forms.SplitContainer splitContainer_right_v;
        private System.Windows.Forms.TabControl tabControl_right_top;
        private System.Windows.Forms.TabControl tabControl_right_bottom;
    }
}