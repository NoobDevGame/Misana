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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayerView));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.button_addLayer = new System.Windows.Forms.ToolStripButton();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripButton_toggleEntities = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
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
            // listView
            // 
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(0, 25);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(284, 236);
            this.listView.TabIndex = 2;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView_ItemCheck);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 48;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 173;
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
            // LayerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.toolStrip1);
            this.Name = "LayerView";
            this.Text = "LayerView";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton button_addLayer;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripButton toolStripButton_toggleEntities;
    }
}