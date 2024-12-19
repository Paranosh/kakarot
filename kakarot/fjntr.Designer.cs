namespace kakarot
{
    partial class fjntr
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fjntr));
            panel1 = new Panel();
            checkBox3 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox1 = new CheckBox();
            label1 = new Label();
            textBox1 = new TextBox();
            dataGridView1 = new DataGridView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            herramientasToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            reposToolStripMenuItem = new ToolStripMenuItem();
            filehunterToolStripMenuItem = new ToolStripMenuItem();
            verNovedadesToolStripMenuItem = new ToolStripMenuItem();
            toolStripComboBox3 = new ToolStripComboBox();
            toolStripSeparator1 = new ToolStripSeparator();
            webMSXToolStripMenuItem = new ToolStripMenuItem();
            descargarSeleccionToolStripMenuItem = new ToolStripMenuItem();
            enviarAMsxToolStripMenuItem = new ToolStripMenuItem();
            toolStripComboBox1 = new ToolStripComboBox();
            baudrateToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            opcionesToolStripMenuItem = new ToolStripMenuItem();
            descomprimirDespuesDeDescargarToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            buscarToolStripMenuItem1 = new ToolStripMenuItem();
            dataGridView2 = new DataGridView();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            statusStrip1 = new StatusStrip();
            notifyIcon = new NotifyIcon(components);
            convertirAToolStripMenuItem = new ToolStripMenuItem();
            dSKROMToolStripMenuItem = new ToolStripMenuItem();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ControlDarkDark;
            panel1.Controls.Add(checkBox3);
            panel1.Controls.Add(checkBox2);
            panel1.Controls.Add(checkBox1);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(textBox1);
            panel1.Location = new Point(570, 477);
            panel1.Name = "panel1";
            panel1.Size = new Size(252, 58);
            panel1.TabIndex = 7;
            panel1.Visible = false;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Checked = true;
            checkBox3.CheckState = CheckState.Checked;
            checkBox3.Location = new Point(202, 3);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(41, 19);
            checkBox3.TabIndex = 4;
            checkBox3.Text = "Url";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            checkBox3.Click += checkBox3_Click;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(103, 4);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(58, 19);
            checkBox2.TabIndex = 3;
            checkBox2.Text = "Status";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            checkBox2.Click += checkBox2_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(7, 4);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(50, 19);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Date";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            checkBox1.Click += checkBox1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(103, 4);
            label1.Name = "label1";
            label1.Size = new Size(49, 15);
            label1.TabIndex = 1;
            label1.Text = "FilePath";
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.Menu;
            textBox1.Location = new Point(8, 26);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(235, 23);
            textBox1.TabIndex = 0;
            textBox1.KeyDown += textBox1_KeyDown;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(864, 571);
            dataGridView1.TabIndex = 8;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { herramientasToolStripMenuItem, toolStripSeparator3, reposToolStripMenuItem, toolStripSeparator1, webMSXToolStripMenuItem, descargarSeleccionToolStripMenuItem, enviarAMsxToolStripMenuItem, toolStripSeparator4, opcionesToolStripMenuItem, toolStripSeparator2, buscarToolStripMenuItem1 });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(181, 204);
            // 
            // herramientasToolStripMenuItem
            // 
            herramientasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { convertirAToolStripMenuItem });
            herramientasToolStripMenuItem.Name = "herramientasToolStripMenuItem";
            herramientasToolStripMenuItem.Size = new Size(180, 22);
            herramientasToolStripMenuItem.Text = "Herramientas";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(177, 6);
            // 
            // reposToolStripMenuItem
            // 
            reposToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { filehunterToolStripMenuItem });
            reposToolStripMenuItem.Name = "reposToolStripMenuItem";
            reposToolStripMenuItem.Size = new Size(180, 22);
            reposToolStripMenuItem.Text = "Repos";
            // 
            // filehunterToolStripMenuItem
            // 
            filehunterToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { verNovedadesToolStripMenuItem, toolStripComboBox3 });
            filehunterToolStripMenuItem.Name = "filehunterToolStripMenuItem";
            filehunterToolStripMenuItem.Size = new Size(132, 22);
            filehunterToolStripMenuItem.Text = "File-hunter";
            // 
            // verNovedadesToolStripMenuItem
            // 
            verNovedadesToolStripMenuItem.CheckOnClick = true;
            verNovedadesToolStripMenuItem.Name = "verNovedadesToolStripMenuItem";
            verNovedadesToolStripMenuItem.Size = new Size(181, 22);
            verNovedadesToolStripMenuItem.Text = "Ver novedades";
            verNovedadesToolStripMenuItem.CheckedChanged += verNovedadesToolStripMenuItem_CheckedChanged;
            // 
            // toolStripComboBox3
            // 
            toolStripComboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            toolStripComboBox3.Items.AddRange(new object[] { "Filtro rapido", "Games/MSX1", "Games/MSX2", "Games/MSX2+", "Demos", "Music", "Books" });
            toolStripComboBox3.Name = "toolStripComboBox3";
            toolStripComboBox3.Size = new Size(121, 23);
            toolStripComboBox3.SelectedIndexChanged += toolStripComboBox3_SelectedIndexChanged;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // webMSXToolStripMenuItem
            // 
            webMSXToolStripMenuItem.Name = "webMSXToolStripMenuItem";
            webMSXToolStripMenuItem.Size = new Size(180, 22);
            webMSXToolStripMenuItem.Text = "WebMSX";
            webMSXToolStripMenuItem.Click += webMSXToolStripMenuItem_Click;
            // 
            // descargarSeleccionToolStripMenuItem
            // 
            descargarSeleccionToolStripMenuItem.Name = "descargarSeleccionToolStripMenuItem";
            descargarSeleccionToolStripMenuItem.Size = new Size(180, 22);
            descargarSeleccionToolStripMenuItem.Text = "Descargar seleccion";
            descargarSeleccionToolStripMenuItem.Click += descargarSeleccionToolStripMenuItem_Click;
            // 
            // enviarAMsxToolStripMenuItem
            // 
            enviarAMsxToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripComboBox1, baudrateToolStripMenuItem });
            enviarAMsxToolStripMenuItem.Name = "enviarAMsxToolStripMenuItem";
            enviarAMsxToolStripMenuItem.Size = new Size(180, 22);
            enviarAMsxToolStripMenuItem.Text = "Enviar a MSX";
            // 
            // toolStripComboBox1
            // 
            toolStripComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            toolStripComboBox1.Name = "toolStripComboBox1";
            toolStripComboBox1.Size = new Size(121, 23);
            // 
            // baudrateToolStripMenuItem
            // 
            baudrateToolStripMenuItem.Name = "baudrateToolStripMenuItem";
            baudrateToolStripMenuItem.Size = new Size(181, 22);
            baudrateToolStripMenuItem.Text = "baudrate";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(177, 6);
            // 
            // opcionesToolStripMenuItem
            // 
            opcionesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { descomprimirDespuesDeDescargarToolStripMenuItem });
            opcionesToolStripMenuItem.Name = "opcionesToolStripMenuItem";
            opcionesToolStripMenuItem.Size = new Size(180, 22);
            opcionesToolStripMenuItem.Text = "Opciones";
            // 
            // descomprimirDespuesDeDescargarToolStripMenuItem
            // 
            descomprimirDespuesDeDescargarToolStripMenuItem.CheckOnClick = true;
            descomprimirDespuesDeDescargarToolStripMenuItem.Name = "descomprimirDespuesDeDescargarToolStripMenuItem";
            descomprimirDespuesDeDescargarToolStripMenuItem.Size = new Size(265, 22);
            descomprimirDespuesDeDescargarToolStripMenuItem.Text = "Descomprimir despues de descargar";
            descomprimirDespuesDeDescargarToolStripMenuItem.Click += descomprimirDespuesDeDescargarToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(177, 6);
            // 
            // buscarToolStripMenuItem1
            // 
            buscarToolStripMenuItem1.CheckOnClick = true;
            buscarToolStripMenuItem1.Name = "buscarToolStripMenuItem1";
            buscarToolStripMenuItem1.Size = new Size(180, 22);
            buscarToolStripMenuItem1.Text = "Buscar";
            buscarToolStripMenuItem1.CheckedChanged += buscarToolStripMenuItem1_CheckedChanged;
            // 
            // dataGridView2
            // 
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.AllowUserToResizeRows = false;
            dataGridView2.BackgroundColor = SystemColors.ControlDarkDark;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.ContextMenuStrip = contextMenuStrip1;
            dataGridView2.Dock = DockStyle.Fill;
            dataGridView2.Location = new Point(0, 0);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.ReadOnly = true;
            dataGridView2.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.RowHeadersWidth = 62;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.Size = new Size(864, 571);
            dataGridView2.TabIndex = 9;
            dataGridView2.Visible = false;
            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            toolStripStatusLabel1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 549);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(864, 22);
            statusStrip1.TabIndex = 3;
            // 
            // notifyIcon
            // 
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "Kakarot";
            notifyIcon.Visible = true;
            // 
            // convertirAToolStripMenuItem
            // 
            convertirAToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { dSKROMToolStripMenuItem });
            convertirAToolStripMenuItem.Name = "convertirAToolStripMenuItem";
            convertirAToolStripMenuItem.Size = new Size(180, 22);
            convertirAToolStripMenuItem.Text = "Convertir a..";
            // 
            // dSKROMToolStripMenuItem
            // 
            dSKROMToolStripMenuItem.Name = "dSKROMToolStripMenuItem";
            dSKROMToolStripMenuItem.Size = new Size(180, 22);
            dSKROMToolStripMenuItem.Text = "DSK -> ROM";
            dSKROMToolStripMenuItem.Click += dSKROMToolStripMenuItem_Click;
            // 
            // fjntr
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(864, 571);
            Controls.Add(panel1);
            Controls.Add(statusStrip1);
            Controls.Add(dataGridView1);
            Controls.Add(dataGridView2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "fjntr";
            Text = "Kakarot";
            FormClosing += fjntr_FormClosing;
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panel1;
        private TextBox textBox1;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem buscarToolStripMenuItem1;
        private ToolStripMenuItem reposToolStripMenuItem;
        private ToolStripMenuItem filehunterToolStripMenuItem;
        private ToolStripMenuItem verNovedadesToolStripMenuItem;
        private Label label1;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private ToolStripMenuItem descargarSeleccionToolStripMenuItem;
        private ToolStripMenuItem herramientasToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem enviarAMsxToolStripMenuItem;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private StatusStrip statusStrip1;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem opcionesToolStripMenuItem;
        private ToolStripMenuItem baudrateToolStripMenuItem;
        private ToolStripMenuItem descomprimirDespuesDeDescargarToolStripMenuItem;
        private ToolStripMenuItem webMSXToolStripMenuItem;
        private ToolStripComboBox toolStripComboBox3;
        private NotifyIcon notifyIcon;
        private ToolStripMenuItem convertirAToolStripMenuItem;
        private ToolStripMenuItem dSKROMToolStripMenuItem;
    }
}
