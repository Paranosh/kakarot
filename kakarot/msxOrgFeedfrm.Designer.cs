namespace kakarot
{
    partial class msxOrgFeedfrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(msxOrgFeedfrm));
            listBox1 = new ListBox();
            linkLabel1 = new LinkLabel();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(2, 0);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(623, 210);
            listBox1.TabIndex = 0;
            listBox1.Click += listBox1_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.Dock = DockStyle.Bottom;
            linkLabel1.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            linkLabel1.Location = new Point(0, 249);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(626, 15);
            linkLabel1.TabIndex = 1;
            linkLabel1.TextAlign = ContentAlignment.MiddleCenter;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(2, 220);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(623, 23);
            textBox1.TabIndex = 2;
            // 
            // msxOrgFeedfrm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(626, 264);
            Controls.Add(textBox1);
            Controls.Add(linkLabel1);
            Controls.Add(listBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "msxOrgFeedfrm";
            Text = "FEED msx.org <noticias>";
            Load += msxOrgFeedfrm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox1;
        private LinkLabel linkLabel1;
        private TextBox textBox1;
    }
}