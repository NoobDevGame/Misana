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
            this.menuItem_view = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_view_log = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_about = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_about_about = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_about_credits = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_about_help = new System.Windows.Forms.ToolStripMenuItem();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.menuItem_view_mapTree = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 519);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1251, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_file,
            this.menuItem_map,
            this.menuItem_view,
            this.menuItem_about});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1251, 24);
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
            this.menuItem_file_import.Name = "menuItem_file_import";
            this.menuItem_file_import.Size = new System.Drawing.Size(112, 22);
            this.menuItem_file_import.Text = "Import";
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
            // menuItem_view
            // 
            this.menuItem_view.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_view_mapTree,
            this.menuItem_view_log});
            this.menuItem_view.Name = "menuItem_view";
            this.menuItem_view.Size = new System.Drawing.Size(44, 20);
            this.menuItem_view.Text = "View";
            // 
            // menuItem_view_log
            // 
            this.menuItem_view_log.CheckOnClick = true;
            this.menuItem_view_log.Name = "menuItem_view_log";
            this.menuItem_view_log.Size = new System.Drawing.Size(152, 22);
            this.menuItem_view_log.Text = "Log";
            this.menuItem_view_log.CheckStateChanged += new System.EventHandler(this.menuItem_view_log_CheckStateChanged);
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
            // dockPanel
            // 
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Location = new System.Drawing.Point(0, 24);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(1251, 495);
            this.dockPanel.TabIndex = 2;
            // 
            // menuItem_view_mapTree
            // 
            this.menuItem_view_mapTree.Name = "menuItem_view_mapTree";
            this.menuItem_view_mapTree.Size = new System.Drawing.Size(152, 22);
            this.menuItem_view_mapTree.Text = "MapTree";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1251, 541);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStripMenuItem menuItem_file;
        private System.Windows.Forms.ToolStripMenuItem menuItem_map;
        private System.Windows.Forms.ToolStripMenuItem menuItem_view;
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
        private System.Windows.Forms.ToolStripMenuItem menuItem_view_log;
        private System.Windows.Forms.ToolStripMenuItem menuItem_view_mapTree;
    }
}