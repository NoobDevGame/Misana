namespace Misana.Editor
{
    partial class Editor
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neueAreaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.convertTiledMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.treeView_maps = new System.Windows.Forms.TreeView();
            this.rightSplit = new System.Windows.Forms.SplitContainer();
            this.tileSplit = new System.Windows.Forms.SplitContainer();
            this.tabControl_tileset = new System.Windows.Forms.TabControl();
            this.propertyGrid_tile = new System.Windows.Forms.PropertyGrid();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).BeginInit();
            this.rightSplit.Panel2.SuspendLayout();
            this.rightSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tileSplit)).BeginInit();
            this.tileSplit.Panel1.SuspendLayout();
            this.tileSplit.Panel2.SuspendLayout();
            this.tileSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mapToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1048, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(109, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // mapToolStripMenuItem
            // 
            this.mapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.neueAreaToolStripMenuItem,
            this.toolStripMenuItem2,
            this.convertTiledMapToolStripMenuItem});
            this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
            this.mapToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.mapToolStripMenuItem.Text = "Map";
            // 
            // neueAreaToolStripMenuItem
            // 
            this.neueAreaToolStripMenuItem.Name = "neueAreaToolStripMenuItem";
            this.neueAreaToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.neueAreaToolStripMenuItem.Text = "Neue Area";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(169, 6);
            // 
            // convertTiledMapToolStripMenuItem
            // 
            this.convertTiledMapToolStripMenuItem.Name = "convertTiledMapToolStripMenuItem";
            this.convertTiledMapToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.convertTiledMapToolStripMenuItem.Text = "Convert Tiled Map";
            this.convertTiledMapToolStripMenuItem.Click += new System.EventHandler(this.convertTiledMapToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.creditsToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // creditsToolStripMenuItem
            // 
            this.creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            this.creditsToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.creditsToolStripMenuItem.Text = "Credits";
            this.creditsToolStripMenuItem.Click += new System.EventHandler(this.creditsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(111, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // mainSplit
            // 
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.Location = new System.Drawing.Point(0, 24);
            this.mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            this.mainSplit.Panel1.Controls.Add(this.treeView_maps);
            // 
            // mainSplit.Panel2
            // 
            this.mainSplit.Panel2.Controls.Add(this.rightSplit);
            this.mainSplit.Size = new System.Drawing.Size(1048, 571);
            this.mainSplit.SplitterDistance = 221;
            this.mainSplit.TabIndex = 1;
            // 
            // treeView_maps
            // 
            this.treeView_maps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_maps.Location = new System.Drawing.Point(0, 0);
            this.treeView_maps.Name = "treeView_maps";
            this.treeView_maps.Size = new System.Drawing.Size(221, 571);
            this.treeView_maps.TabIndex = 0;
            this.treeView_maps.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_maps_AfterSelect);
            // 
            // rightSplit
            // 
            this.rightSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightSplit.Location = new System.Drawing.Point(0, 0);
            this.rightSplit.Name = "rightSplit";
            // 
            // rightSplit.Panel2
            // 
            this.rightSplit.Panel2.Controls.Add(this.tileSplit);
            this.rightSplit.Size = new System.Drawing.Size(823, 571);
            this.rightSplit.SplitterDistance = 625;
            this.rightSplit.TabIndex = 0;
            // 
            // tileSplit
            // 
            this.tileSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.tileSplit.Location = new System.Drawing.Point(0, 0);
            this.tileSplit.Name = "tileSplit";
            this.tileSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // tileSplit.Panel1
            // 
            this.tileSplit.Panel1.Controls.Add(this.tabControl_tileset);
            // 
            // tileSplit.Panel2
            // 
            this.tileSplit.Panel2.Controls.Add(this.propertyGrid_tile);
            this.tileSplit.Size = new System.Drawing.Size(194, 571);
            this.tileSplit.SplitterDistance = 350;
            this.tileSplit.TabIndex = 0;
            // 
            // tabControl_tileset
            // 
            this.tabControl_tileset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_tileset.Location = new System.Drawing.Point(0, 0);
            this.tabControl_tileset.Name = "tabControl_tileset";
            this.tabControl_tileset.SelectedIndex = 0;
            this.tabControl_tileset.Size = new System.Drawing.Size(194, 350);
            this.tabControl_tileset.TabIndex = 0;
            // 
            // propertyGrid_tile
            // 
            this.propertyGrid_tile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid_tile.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid_tile.Name = "propertyGrid_tile";
            this.propertyGrid_tile.Size = new System.Drawing.Size(194, 217);
            this.propertyGrid_tile.TabIndex = 0;
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1048, 595);
            this.Controls.Add(this.mainSplit);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Editor";
            this.Text = "Misana Map Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.rightSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).EndInit();
            this.rightSplit.ResumeLayout(false);
            this.tileSplit.Panel1.ResumeLayout(false);
            this.tileSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tileSplit)).EndInit();
            this.tileSplit.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.TreeView treeView_maps;
        private System.Windows.Forms.SplitContainer rightSplit;
        private System.Windows.Forms.SplitContainer tileSplit;
        private System.Windows.Forms.TabControl tabControl_tileset;
        private System.Windows.Forms.PropertyGrid propertyGrid_tile;
        private System.Windows.Forms.ToolStripMenuItem neueAreaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creditsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem convertTiledMapToolStripMenuItem;
    }
}

