namespace Misana.Editor.Forms.MDI
{
    partial class LayerView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayerView));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.button_addLayer = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_toggleEntities = new System.Windows.Forms.ToolStripButton();
            this.listView = new System.Windows.Forms.ListView();
            this.contextMenuStrip_layer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_rename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_remove = new System.Windows.Forms.ToolStripMenuItem();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip_layer.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.button_addLayer,
            this.toolStripButton_toggleEntities});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(284, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // button_addLayer
            // 
            this.button_addLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.button_addLayer.Image = ((System.Drawing.Image)(resources.GetObject("button_addLayer.Image")));
            this.button_addLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_addLayer.Name = "button_addLayer";
            this.button_addLayer.Size = new System.Drawing.Size(64, 22);
            this.button_addLayer.Text = "Add Layer";
            this.button_addLayer.Click += new System.EventHandler(this.button_addLayer_Click);
            // 
            // toolStripButton_toggleEntities
            // 
            this.toolStripButton_toggleEntities.CheckOnClick = true;
            this.toolStripButton_toggleEntities.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_toggleEntities.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_toggleEntities.Image")));
            this.toolStripButton_toggleEntities.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_toggleEntities.Name = "toolStripButton_toggleEntities";
            this.toolStripButton_toggleEntities.Size = new System.Drawing.Size(88, 22);
            this.toolStripButton_toggleEntities.Text = "Toggle Entities";
            this.toolStripButton_toggleEntities.CheckedChanged += new System.EventHandler(this.toolStripButton_toggleEntities_CheckedChanged);
            // 
            // listView
            // 
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.LabelEdit = true;
            this.listView.Location = new System.Drawing.Point(0, 25);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(284, 236);
            this.listView.TabIndex = 2;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView_AfterLabelEdit);
            this.listView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView_ItemCheck);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseClick);
            // 
            // contextMenuStrip_layer
            // 
            this.contextMenuStrip_layer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_rename,
            this.toolStripSeparator1,
            this.toolStripMenuItem_remove});
            this.contextMenuStrip_layer.Name = "contextMenuStrip_layer";
            this.contextMenuStrip_layer.Size = new System.Drawing.Size(118, 54);
            // 
            // toolStripMenuItem_rename
            // 
            this.toolStripMenuItem_rename.Name = "toolStripMenuItem_rename";
            this.toolStripMenuItem_rename.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItem_rename.Text = "Rename";
            this.toolStripMenuItem_rename.Click += new System.EventHandler(this.toolStripMenuItem_rename_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(114, 6);
            // 
            // toolStripMenuItem_remove
            // 
            this.toolStripMenuItem_remove.Name = "toolStripMenuItem_remove";
            this.toolStripMenuItem_remove.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItem_remove.Text = "Remove";
            this.toolStripMenuItem_remove.Click += new System.EventHandler(this.toolStripMenuItem_remove_Click);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 151;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ID";
            // 
            // LayerView
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "LayerView";
            this.Text = "LayerView";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip_layer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton button_addLayer;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ToolStripButton toolStripButton_toggleEntities;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_layer;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_rename;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_remove;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}