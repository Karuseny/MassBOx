namespace module
{
    partial class WifiView
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
            this.listView7 = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView7
            // 
            this.listView7.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView7.AllowColumnReorder = true;
            this.listView7.AllowDrop = true;
            this.listView7.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.listView7.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12});
            this.listView7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView7.FullRowSelect = true;
            this.listView7.GridLines = true;
            this.listView7.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView7.HideSelection = false;
            this.listView7.LabelWrap = false;
            this.listView7.Location = new System.Drawing.Point(0, 0);
            this.listView7.MultiSelect = false;
            this.listView7.Name = "listView7";
            this.listView7.ShowGroups = false;
            this.listView7.Size = new System.Drawing.Size(935, 389);
            this.listView7.TabIndex = 3;
            this.listView7.UseCompatibleStateImageBehavior = false;
            this.listView7.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Hex";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ConnectionType";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "ConnectionMode";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Authentication";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Encryption";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "UseOneX";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "KeyType";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Protected";
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "KeyMaterial";
            this.columnHeader11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "EnableRandomization";
            // 
            // WifiView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(935, 389);
            this.Controls.Add(this.listView7);
            this.DoubleBuffered = true;
            this.ImeMode = System.Windows.Forms.ImeMode.Katakana;
            this.MaximizeBox = false;
            this.Name = "WifiView";
            this.Opacity = 0.95D;
            this.ShowIcon = false;
            this.Text = "Wifi查看器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WifiView_FormClosing);
            this.Load += new System.EventHandler(this.WifiView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListView listView7;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
    }
}