﻿namespace kakarot
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
            convertirAToolStripMenuItem = new ToolStripMenuItem();
            dSKROMToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            bASROMToolStripMenuItem = new ToolStripMenuItem();
            rOMCASToolStripMenuItem = new ToolStripMenuItem();
            dragDropdskExploresrToolStripMenuItem = new ToolStripMenuItem();
            aplicarParcheIPSToolStripMenuItem = new ToolStripMenuItem();
            ConcatoolStripMenuItem = new ToolStripMenuItem();
            verSHA1ToolStripMenuItem = new ToolStripMenuItem();
            EnviarAMSXMenuItem = new ToolStripMenuItem();
            toolStripComboBox2 = new ToolStripComboBox();
            toolStripComboBox4 = new ToolStripComboBox();
            enviaArchivoSeleccionadoToolStripMenuItem = new ToolStripMenuItem();
            enviaArchivoLocalToolStripMenuItem = new ToolStripMenuItem();
            MuestraImagentoolStripMenuItem1 = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            reposToolStripMenuItem = new ToolStripMenuItem();
            filehunterToolStripMenuItem = new ToolStripMenuItem();
            verNovedadesToolStripMenuItem = new ToolStripMenuItem();
            informarDeActualizacionesToolStripMenuItem = new ToolStripMenuItem();
            toolStripComboBox3 = new ToolStripComboBox();
            MSXScanstoolStripMenuItem4 = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            webMSXToolStripMenuItem = new ToolStripMenuItem();
            descargarSeleccionToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            opcionesToolStripMenuItem = new ToolStripMenuItem();
            descomprimirDespuesDeDescargarToolStripMenuItem = new ToolStripMenuItem();
            SetOpenMSXPathtoolStripMenuItem = new ToolStripMenuItem();
            permitirMultiplesInstanciasToolStripMenuItem = new ToolStripMenuItem();
            IdiomaMenuItem = new ToolStripMenuItem();
            IdiomaSpatoolStripMenuItem = new ToolStripMenuItem();
            IdiomaEngtoolStripMenuItem = new ToolStripMenuItem();
            integrarLanzadorConOSToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            buscarToolStripMenuItem1 = new ToolStripMenuItem();
            copiarToolStripMenuItem = new ToolStripMenuItem();
            dataGridView2 = new DataGridView();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            statusStrip1 = new StatusStrip();
            notifyIcon = new NotifyIcon(components);
            listBox1 = new ListBox();
            listBox2 = new ListBox();
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
            textBox1.TextChanged += textBox1_TextChanged;
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
            dataGridView1.Size = new Size(864, 581);
            dataGridView1.TabIndex = 8;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(24, 24);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { herramientasToolStripMenuItem, MuestraImagentoolStripMenuItem1, toolStripSeparator3, reposToolStripMenuItem, toolStripSeparator1, webMSXToolStripMenuItem, descargarSeleccionToolStripMenuItem, toolStripSeparator4, opcionesToolStripMenuItem, toolStripSeparator2, buscarToolStripMenuItem1, copiarToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(179, 204);
            // 
            // herramientasToolStripMenuItem
            // 
            herramientasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { convertirAToolStripMenuItem, dragDropdskExploresrToolStripMenuItem, aplicarParcheIPSToolStripMenuItem, ConcatoolStripMenuItem, verSHA1ToolStripMenuItem, EnviarAMSXMenuItem });
            herramientasToolStripMenuItem.Name = "herramientasToolStripMenuItem";
            herramientasToolStripMenuItem.Size = new Size(178, 22);
            herramientasToolStripMenuItem.Text = "Herramientas";
            // 
            // convertirAToolStripMenuItem
            // 
            convertirAToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { dSKROMToolStripMenuItem, toolStripMenuItem3, bASROMToolStripMenuItem, rOMCASToolStripMenuItem });
            convertirAToolStripMenuItem.Name = "convertirAToolStripMenuItem";
            convertirAToolStripMenuItem.Size = new Size(177, 22);
            convertirAToolStripMenuItem.Text = "Convertir a..";
            // 
            // dSKROMToolStripMenuItem
            // 
            dSKROMToolStripMenuItem.Name = "dSKROMToolStripMenuItem";
            dSKROMToolStripMenuItem.Size = new Size(142, 22);
            dSKROMToolStripMenuItem.Text = "DSK -> ROM";
            dSKROMToolStripMenuItem.Click += dSKROMToolStripMenuItem_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(142, 22);
            toolStripMenuItem3.Text = "BAS -> ASCII";
            toolStripMenuItem3.Click += toolStripMenuItem3_Click;
            // 
            // bASROMToolStripMenuItem
            // 
            bASROMToolStripMenuItem.Name = "bASROMToolStripMenuItem";
            bASROMToolStripMenuItem.Size = new Size(142, 22);
            bASROMToolStripMenuItem.Text = "BAS -> ROM";
            bASROMToolStripMenuItem.Visible = false;
            bASROMToolStripMenuItem.Click += bASROMToolStripMenuItem_Click;
            // 
            // rOMCASToolStripMenuItem
            // 
            rOMCASToolStripMenuItem.Name = "rOMCASToolStripMenuItem";
            rOMCASToolStripMenuItem.Size = new Size(142, 22);
            rOMCASToolStripMenuItem.Text = "ROM -> CAS";
            rOMCASToolStripMenuItem.Click += rOMCASToolStripMenuItem_Click;
            // 
            // dragDropdskExploresrToolStripMenuItem
            // 
            dragDropdskExploresrToolStripMenuItem.Name = "dragDropdskExploresrToolStripMenuItem";
            dragDropdskExploresrToolStripMenuItem.Size = new Size(177, 22);
            dragDropdskExploresrToolStripMenuItem.Text = "DragDrop .DSK Ex";
            dragDropdskExploresrToolStripMenuItem.Click += dragDropdskExploresrToolStripMenuItem_Click;
            // 
            // aplicarParcheIPSToolStripMenuItem
            // 
            aplicarParcheIPSToolStripMenuItem.Name = "aplicarParcheIPSToolStripMenuItem";
            aplicarParcheIPSToolStripMenuItem.Size = new Size(177, 22);
            aplicarParcheIPSToolStripMenuItem.Text = "Parchear ROM (IPS)";
            aplicarParcheIPSToolStripMenuItem.Click += aplicarParcheIPSToolStripMenuItem_Click;
            // 
            // ConcatoolStripMenuItem
            // 
            ConcatoolStripMenuItem.Name = "ConcatoolStripMenuItem";
            ConcatoolStripMenuItem.Size = new Size(177, 22);
            ConcatoolStripMenuItem.Text = "Concatenar";
            ConcatoolStripMenuItem.Click += toolStripMenuItem2_Click;
            // 
            // verSHA1ToolStripMenuItem
            // 
            verSHA1ToolStripMenuItem.Name = "verSHA1ToolStripMenuItem";
            verSHA1ToolStripMenuItem.Size = new Size(177, 22);
            verSHA1ToolStripMenuItem.Text = "SHA1";
            verSHA1ToolStripMenuItem.Click += verSHA1ToolStripMenuItem_Click;
            // 
            // EnviarAMSXMenuItem
            // 
            EnviarAMSXMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripComboBox2, toolStripComboBox4, enviaArchivoSeleccionadoToolStripMenuItem, enviaArchivoLocalToolStripMenuItem });
            EnviarAMSXMenuItem.Name = "EnviarAMSXMenuItem";
            EnviarAMSXMenuItem.Size = new Size(177, 22);
            EnviarAMSXMenuItem.Text = "Enviar a MSX";
            EnviarAMSXMenuItem.Visible = false;
            // 
            // toolStripComboBox2
            // 
            toolStripComboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            toolStripComboBox2.Name = "toolStripComboBox2";
            toolStripComboBox2.Size = new Size(121, 23);
            // 
            // toolStripComboBox4
            // 
            toolStripComboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
            toolStripComboBox4.Items.AddRange(new object[] { "57600", "19200", "9600", "4800", "2400" });
            toolStripComboBox4.Name = "toolStripComboBox4";
            toolStripComboBox4.Size = new Size(121, 23);
            // 
            // enviaArchivoSeleccionadoToolStripMenuItem
            // 
            enviaArchivoSeleccionadoToolStripMenuItem.Name = "enviaArchivoSeleccionadoToolStripMenuItem";
            enviaArchivoSeleccionadoToolStripMenuItem.Size = new Size(216, 22);
            enviaArchivoSeleccionadoToolStripMenuItem.Text = "Envia archivo seleccionado";
            enviaArchivoSeleccionadoToolStripMenuItem.Click += enviaArchivoSeleccionadoToolStripMenuItem_Click;
            // 
            // enviaArchivoLocalToolStripMenuItem
            // 
            enviaArchivoLocalToolStripMenuItem.Name = "enviaArchivoLocalToolStripMenuItem";
            enviaArchivoLocalToolStripMenuItem.Size = new Size(216, 22);
            enviaArchivoLocalToolStripMenuItem.Text = "Envia archivo local";
            enviaArchivoLocalToolStripMenuItem.Click += enviaArchivoLocalToolStripMenuItem_Click;
            // 
            // MuestraImagentoolStripMenuItem1
            // 
            MuestraImagentoolStripMenuItem1.Name = "MuestraImagentoolStripMenuItem1";
            MuestraImagentoolStripMenuItem1.Size = new Size(178, 22);
            MuestraImagentoolStripMenuItem1.Text = "Abrir seleccion";
            MuestraImagentoolStripMenuItem1.Click += MuestraImagentoolStripMenuItem1_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(175, 6);
            // 
            // reposToolStripMenuItem
            // 
            reposToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { filehunterToolStripMenuItem, MSXScanstoolStripMenuItem4 });
            reposToolStripMenuItem.Name = "reposToolStripMenuItem";
            reposToolStripMenuItem.Size = new Size(178, 22);
            reposToolStripMenuItem.Text = "Repos";
            // 
            // filehunterToolStripMenuItem
            // 
            filehunterToolStripMenuItem.Checked = true;
            filehunterToolStripMenuItem.CheckState = CheckState.Checked;
            filehunterToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { verNovedadesToolStripMenuItem, informarDeActualizacionesToolStripMenuItem, toolStripComboBox3 });
            filehunterToolStripMenuItem.Name = "filehunterToolStripMenuItem";
            filehunterToolStripMenuItem.Size = new Size(133, 22);
            filehunterToolStripMenuItem.Text = "File-hunter";
            filehunterToolStripMenuItem.Click += filehunterToolStripMenuItem_Click;
            // 
            // verNovedadesToolStripMenuItem
            // 
            verNovedadesToolStripMenuItem.CheckOnClick = true;
            verNovedadesToolStripMenuItem.Name = "verNovedadesToolStripMenuItem";
            verNovedadesToolStripMenuItem.Size = new Size(219, 22);
            verNovedadesToolStripMenuItem.Text = "Ver novedades";
            verNovedadesToolStripMenuItem.CheckedChanged += verNovedadesToolStripMenuItem_CheckedChanged;
            // 
            // informarDeActualizacionesToolStripMenuItem
            // 
            informarDeActualizacionesToolStripMenuItem.CheckOnClick = true;
            informarDeActualizacionesToolStripMenuItem.Name = "informarDeActualizacionesToolStripMenuItem";
            informarDeActualizacionesToolStripMenuItem.Size = new Size(219, 22);
            informarDeActualizacionesToolStripMenuItem.Text = "Informar de actualizaciones";
            informarDeActualizacionesToolStripMenuItem.CheckedChanged += informarDeActualizacionesToolStripMenuItem_CheckedChanged;
            // 
            // toolStripComboBox3
            // 
            toolStripComboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            toolStripComboBox3.Items.AddRange(new object[] { "Filtro rápido", "Games\\MSX1", "Games\\MSX2", "Games\\MSX2+", "Demos", "Music", "Books" });
            toolStripComboBox3.Name = "toolStripComboBox3";
            toolStripComboBox3.Size = new Size(121, 23);
            toolStripComboBox3.SelectedIndexChanged += toolStripComboBox3_SelectedIndexChanged;
            // 
            // MSXScanstoolStripMenuItem4
            // 
            MSXScanstoolStripMenuItem4.CheckOnClick = true;
            MSXScanstoolStripMenuItem4.Name = "MSXScanstoolStripMenuItem4";
            MSXScanstoolStripMenuItem4.Size = new Size(133, 22);
            MSXScanstoolStripMenuItem4.Text = "MSX-Scans";
            MSXScanstoolStripMenuItem4.Click += toolStripMenuItem4_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(175, 6);
            // 
            // webMSXToolStripMenuItem
            // 
            webMSXToolStripMenuItem.Name = "webMSXToolStripMenuItem";
            webMSXToolStripMenuItem.Size = new Size(178, 22);
            webMSXToolStripMenuItem.Text = "WebMSX";
            webMSXToolStripMenuItem.Click += webMSXToolStripMenuItem_Click;
            // 
            // descargarSeleccionToolStripMenuItem
            // 
            descargarSeleccionToolStripMenuItem.Name = "descargarSeleccionToolStripMenuItem";
            descargarSeleccionToolStripMenuItem.Size = new Size(178, 22);
            descargarSeleccionToolStripMenuItem.Text = "Descargar seleccion";
            descargarSeleccionToolStripMenuItem.Click += descargarSeleccionToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(175, 6);
            // 
            // opcionesToolStripMenuItem
            // 
            opcionesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { descomprimirDespuesDeDescargarToolStripMenuItem, SetOpenMSXPathtoolStripMenuItem, permitirMultiplesInstanciasToolStripMenuItem, IdiomaMenuItem, integrarLanzadorConOSToolStripMenuItem });
            opcionesToolStripMenuItem.Name = "opcionesToolStripMenuItem";
            opcionesToolStripMenuItem.Size = new Size(178, 22);
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
            // SetOpenMSXPathtoolStripMenuItem
            // 
            SetOpenMSXPathtoolStripMenuItem.Name = "SetOpenMSXPathtoolStripMenuItem";
            SetOpenMSXPathtoolStripMenuItem.Size = new Size(265, 22);
            SetOpenMSXPathtoolStripMenuItem.Text = "Establecer ruta para OpenMSX";
            SetOpenMSXPathtoolStripMenuItem.Click += toolStripMenuItem1_Click;
            // 
            // permitirMultiplesInstanciasToolStripMenuItem
            // 
            permitirMultiplesInstanciasToolStripMenuItem.CheckOnClick = true;
            permitirMultiplesInstanciasToolStripMenuItem.Name = "permitirMultiplesInstanciasToolStripMenuItem";
            permitirMultiplesInstanciasToolStripMenuItem.Size = new Size(265, 22);
            permitirMultiplesInstanciasToolStripMenuItem.Text = "Permitir multiples instancias";
            permitirMultiplesInstanciasToolStripMenuItem.CheckedChanged += permitirMultiplesInstanciasToolStripMenuItem_CheckedChanged;
            // 
            // IdiomaMenuItem
            // 
            IdiomaMenuItem.DropDownItems.AddRange(new ToolStripItem[] { IdiomaSpatoolStripMenuItem, IdiomaEngtoolStripMenuItem });
            IdiomaMenuItem.Name = "IdiomaMenuItem";
            IdiomaMenuItem.Size = new Size(265, 22);
            IdiomaMenuItem.Text = "Idioma";
            // 
            // IdiomaSpatoolStripMenuItem
            // 
            IdiomaSpatoolStripMenuItem.Name = "IdiomaSpatoolStripMenuItem";
            IdiomaSpatoolStripMenuItem.Size = new Size(115, 22);
            IdiomaSpatoolStripMenuItem.Text = "Español";
            IdiomaSpatoolStripMenuItem.Click += IdiomaSpatoolStripMenuItem_Click;
            // 
            // IdiomaEngtoolStripMenuItem
            // 
            IdiomaEngtoolStripMenuItem.Name = "IdiomaEngtoolStripMenuItem";
            IdiomaEngtoolStripMenuItem.Size = new Size(115, 22);
            IdiomaEngtoolStripMenuItem.Text = "Ingles";
            IdiomaEngtoolStripMenuItem.Click += IdiomaEngtoolStripMenuItem5_Click;
            // 
            // integrarLanzadorConOSToolStripMenuItem
            // 
            integrarLanzadorConOSToolStripMenuItem.CheckOnClick = true;
            integrarLanzadorConOSToolStripMenuItem.Name = "integrarLanzadorConOSToolStripMenuItem";
            integrarLanzadorConOSToolStripMenuItem.Size = new Size(265, 22);
            integrarLanzadorConOSToolStripMenuItem.Text = "Integrar lanzador con OS (W10,W11)";
            integrarLanzadorConOSToolStripMenuItem.CheckedChanged += integrarLanzadorConOSToolStripMenuItem_CheckedChanged;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(175, 6);
            // 
            // buscarToolStripMenuItem1
            // 
            buscarToolStripMenuItem1.CheckOnClick = true;
            buscarToolStripMenuItem1.Name = "buscarToolStripMenuItem1";
            buscarToolStripMenuItem1.Size = new Size(178, 22);
            buscarToolStripMenuItem1.Text = "Buscar";
            buscarToolStripMenuItem1.CheckedChanged += buscarToolStripMenuItem1_CheckedChanged;
            // 
            // copiarToolStripMenuItem
            // 
            copiarToolStripMenuItem.Name = "copiarToolStripMenuItem";
            copiarToolStripMenuItem.Size = new Size(178, 22);
            copiarToolStripMenuItem.Text = "Copiar";
            copiarToolStripMenuItem.Visible = false;
            copiarToolStripMenuItem.Click += copiarToolStripMenuItem_Click;
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
            dataGridView2.Size = new Size(864, 581);
            dataGridView2.TabIndex = 9;
            dataGridView2.Visible = false;
            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(65, 17);
            toolStripStatusLabel1.Text = "Iniciando...";
            toolStripStatusLabel1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 559);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(864, 22);
            statusStrip1.TabIndex = 3;
            // 
            // notifyIcon
            // 
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipText = "La aplicación está en la bandeja del sistema.";
            notifyIcon.BalloonTipTitle = "Aplicación minimizada";
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "Kakarot";
            notifyIcon.Visible = true;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
            // 
            // listBox1
            // 
            listBox1.BackColor = SystemColors.ControlDark;
            listBox1.ContextMenuStrip = contextMenuStrip1;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(0, 0);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(864, 559);
            listBox1.TabIndex = 10;
            listBox1.Visible = false;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            listBox1.DoubleClick += listBox1_DoubleClick;
            // 
            // listBox2
            // 
            listBox2.BackColor = SystemColors.ControlDark;
            listBox2.ContextMenuStrip = contextMenuStrip1;
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(0, 0);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(864, 559);
            listBox2.TabIndex = 11;
            listBox2.Visible = false;
            listBox2.SelectedIndexChanged += listBox2_SelectedIndexChanged;
            listBox2.DoubleClick += listBox2_DoubleClick;
            // 
            // fjntr
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(864, 581);
            Controls.Add(listBox2);
            Controls.Add(listBox1);
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
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private StatusStrip statusStrip1;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem opcionesToolStripMenuItem;
        private ToolStripMenuItem descomprimirDespuesDeDescargarToolStripMenuItem;
        private ToolStripMenuItem webMSXToolStripMenuItem;
        private ToolStripComboBox toolStripComboBox3;
        private NotifyIcon notifyIcon;
        private ToolStripMenuItem convertirAToolStripMenuItem;
        private ToolStripMenuItem dSKROMToolStripMenuItem;
        private ToolStripMenuItem SetOpenMSXPathtoolStripMenuItem;
        private ToolStripMenuItem ConcatoolStripMenuItem;
        private ToolStripMenuItem permitirMultiplesInstanciasToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem3;
        private DataGridView dataGridView3;
        private ListBox listBox1;
        private ToolStripMenuItem MSXScanstoolStripMenuItem4;
        private ToolStripMenuItem informarDeActualizacionesToolStripMenuItem;
        private ToolStripMenuItem rOMCASToolStripMenuItem;
        private ToolStripMenuItem aplicarParcheIPSToolStripMenuItem;
        private ToolStripMenuItem verSHA1ToolStripMenuItem;
        private ToolStripMenuItem EnviarAMSXMenuItem;
        private ToolStripComboBox toolStripComboBox2;
        private ToolStripComboBox toolStripComboBox4;
        private ToolStripMenuItem enviaArchivoSeleccionadoToolStripMenuItem;
        private ToolStripMenuItem enviaArchivoLocalToolStripMenuItem;
        private ToolStripMenuItem dragDropdskExploresrToolStripMenuItem;
        private ToolStripMenuItem IdiomaMenuItem;
        private ToolStripMenuItem IdiomaSpatoolStripMenuItem;
        private ToolStripMenuItem IdiomaEngtoolStripMenuItem;
        private ListBox listBox2;
        private ToolStripMenuItem MuestraImagentoolStripMenuItem1;
        private ToolStripMenuItem bASROMToolStripMenuItem;
        private ToolStripMenuItem copiarToolStripMenuItem;
        private ToolStripMenuItem integrarLanzadorConOSToolStripMenuItem;
    }
}
