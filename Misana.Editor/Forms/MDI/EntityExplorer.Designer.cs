namespace Misana.Editor.Forms.MDI
{
    partial class EntityExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntityExplorer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.button_add = new System.Windows.Forms.ToolStripButton();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_edef = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItem_edit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_remove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip_edef.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.button_add});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(284, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // button_add
            // 
            this.button_add.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.button_add.Image = ((System.Drawing.Image)(resources.GetObject("button_add.Image")));
            this.button_add.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(33, 22);
            this.button_add.Text = "Add";
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(0, 25);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(284, 236);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView_ItemDrag);
            this.listView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 168;
            // 
            // contextMenuStrip_edef
            // 
            this.contextMenuStrip_edef.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_edit,
            this.menuItem_remove});
            this.contextMenuStrip_edef.Name = "contextMenuStrip_edef";
            this.contextMenuStrip_edef.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenuStrip_edef.Size = new System.Drawing.Size(118, 48);
            // 
            // menuItem_edit
            // 
            this.menuItem_edit.Name = "menuItem_edit";
            this.menuItem_edit.Size = new System.Drawing.Size(117, 22);
            this.menuItem_edit.Text = "Edit";
            // 
            // menuItem_remove
            // 
            this.menuItem_remove.Name = "menuItem_remove";
            this.menuItem_remove.Size = new System.Drawing.Size(117, 22);
            this.menuItem_remove.Text = "Remove";
            // 
            // EntityExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "EntityExplorer";
            this.Text = "EntityExplorer";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip_edef.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton button_add;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_edef;
        private System.Windows.Forms.ToolStripMenuItem menuItem_edit;
        private System.Windows.Forms.ToolStripMenuItem menuItem_remove;
    }
}