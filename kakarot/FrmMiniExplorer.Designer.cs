namespace kakarot
{
    partial class FrmMiniExplorer
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
            components = new System.ComponentModel.Container();
            lvwVDFile = new ListView();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            contextMenuStrip1 = new ContextMenuStrip(components);
            nuevoToolStripMenuItem = new ToolStripMenuItem();
            kToolStripMenuItem = new ToolStripMenuItem();
            kToolStripMenuItem1 = new ToolStripMenuItem();
            abrirToolStripMenuItem = new ToolStripMenuItem();
            borrarToolStripMenuItem = new ToolStripMenuItem();
            bgEsperarError = new System.ComponentModel.BackgroundWorker();
            statusStrip1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // lvwVDFile
            // 
            lvwVDFile.AllowDrop = true;
            lvwVDFile.Dock = DockStyle.Fill;
            lvwVDFile.Location = new Point(0, 0);
            lvwVDFile.Margin = new Padding(4, 3, 4, 3);
            lvwVDFile.Name = "lvwVDFile";
            lvwVDFile.Size = new Size(374, 437);
            lvwVDFile.TabIndex = 0;
            lvwVDFile.UseCompatibleStateImageBehavior = false;
            lvwVDFile.ItemDrag += lvwVDFile_ItemDrag;
            lvwVDFile.DragDrop += lvwVDFile_DragDrop;
            lvwVDFile.DragEnter += lvwVDFile_DragEnter;
            lvwVDFile.DragOver += lvwVDFile_DragOver;
            lvwVDFile.DoubleClick += lvwVDFile_DoubleClick;
            lvwVDFile.KeyDown += lvwVDFile_KeyDown;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 415);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(374, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(0, 17);
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { nuevoToolStripMenuItem, abrirToolStripMenuItem, borrarToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(221, 70);
            // 
            // nuevoToolStripMenuItem
            // 
            nuevoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { kToolStripMenuItem, kToolStripMenuItem1 });
            nuevoToolStripMenuItem.Name = "nuevoToolStripMenuItem";
            nuevoToolStripMenuItem.Size = new Size(220, 22);
            nuevoToolStripMenuItem.Text = "Nuevo";
            // 
            // kToolStripMenuItem
            // 
            kToolStripMenuItem.Name = "kToolStripMenuItem";
            kToolStripMenuItem.Size = new Size(98, 22);
            kToolStripMenuItem.Text = "360k";
            // 
            // kToolStripMenuItem1
            // 
            kToolStripMenuItem1.Name = "kToolStripMenuItem1";
            kToolStripMenuItem1.Size = new Size(98, 22);
            kToolStripMenuItem1.Text = "720k";
            // 
            // abrirToolStripMenuItem
            // 
            abrirToolStripMenuItem.Name = "abrirToolStripMenuItem";
            abrirToolStripMenuItem.Size = new Size(220, 22);
            abrirToolStripMenuItem.Text = "Abrir .dsk";
            // 
            // borrarToolStripMenuItem
            // 
            borrarToolStripMenuItem.Name = "borrarToolStripMenuItem";
            borrarToolStripMenuItem.Size = new Size(220, 22);
            borrarToolStripMenuItem.Text = "Borrar archivo seleccionado";
            // 
            // bgEsperarError
            // 
            bgEsperarError.DoWork += bgEsperarError_DoWork;
            bgEsperarError.ProgressChanged += bgEsperarError_ProgressChanged;
            bgEsperarError.RunWorkerCompleted += bgEsperarError_RunWorkerCompleted;
            // 
            // FrmMiniExplorer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(374, 437);
            Controls.Add(statusStrip1);
            Controls.Add(lvwVDFile);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "FrmMiniExplorer";
            Text = "Mini<Drag&DRop>Explorer";
            TopMost = true;
            FormClosed += FrmMiniExplorer_FormClosed;
            Load += FrmMiniExplorer_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView lvwVDFile;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem nuevoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem abrirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borrarToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgEsperarError;
    }
}