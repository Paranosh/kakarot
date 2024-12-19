using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using FastMember;
using System.Configuration;
using System.IO.Ports;
using System.IO.Compression;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Resources;
using System;
using System.Diagnostics.Eventing.Reader;


namespace kakarot
{

    public partial class fjntr : Form
    {
        public fjntr()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Resize += Form1_Resize;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

        }


        String PathOpenMSX, TipoDeMApper;
        private System.Windows.Forms.Timer fadeTimer;
        private float opacityIncrement;
        private bool fadeIn;
        private DataTable _dataTableDV1;
        private DataTable _dataTableDV2;
        string listado = "", updatelistado = "", archivosdescargados = "Descargado ";
        List<FileData> fileList, fileListUpdate;
        int ContadorDescargas = 0;


        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // Restaurar la ventana al hacer doble clic en el ícono
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        static string FindApplicationInCommonPaths(string appName)
        {
            // Directorios comunes donde se instalan aplicaciones
            string[] commonPaths = new[]
            {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        };

            foreach (string path in commonPaths)
            {
                // Construir la ruta completa donde podría estar el ejecutable
                string potentialPath = Path.Combine(path, appName, $"{appName}.exe");

                if (File.Exists(potentialPath))
                {
                    return Path.GetDirectoryName(potentialPath);
                }
            }
            return null;
        }
        static string isOpenMSXInstalled(string appName)
        {
            string installPath = FindApplicationInCommonPaths(appName);

            if (!string.IsNullOrEmpty(installPath))
            {
                return installPath;
            }
            else
            {
                return null;
            }

        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Asegurar que el panel esté siempre en la esquina inferior derecha
            panel1.Left = this.ClientSize.Width - panel1.Width - 50;  // 10px de margen de la derecha
            panel1.Top = this.ClientSize.Height - panel1.Height - 50; // 10px de margen inferior
            if (this.WindowState == FormWindowState.Minimized)
            {
                // Mostrar el ícono en la bandeja del sistema
                notifyIcon.Visible = true;

                // Ocultar la ventana principal
                this.Hide();

                // Mostrar un mensaje de notificación
                notifyIcon.ShowBalloonTip(1000, "Aplicación minimizada", "La aplicación está en la bandeja del sistema.", ToolTipIcon.Info);
            }
        }
        private async Task TaskDownloadFile(string Filename, string Uri)
        {
            WebClient client = new WebClient();
            client.DownloadFileCompleted += DownloadFileCompleted(Filename);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
            await client.DownloadFileTaskAsync(Uri, Filename);
        }

        public AsyncCompletedEventHandler DownloadFileCompleted(string filename)
        {
            Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
            {

                var _filename = filename;
                if (e.Error != null)
                {
                    throw e.Error;
                }
                else
                {
                    ContadorDescargas++;
                    //terminó
                    if (filename == "sha1sums.txt")
                    {
                        listado = File.ReadAllText("sha1sums.txt");
                        File.Delete(filename);
                        // Separar el string en líneas
                        string[] lines = listado.Trim().Split('\n');

                        // Crear una lista para almacenar los objetos
                        fileList = new List<FileData>();

                        foreach (var line in lines)
                        {
                            // Separar la línea en dos partes: Hash y FilePath
                            string[] parts = line.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

                            // Si la línea tiene dos partes, las asignamos a un objeto FileData
                            if (parts.Length == 2)
                            {
                                fileList.Add(new FileData
                                {
                                    Hash = parts[0].Trim(),
                                    FilePath = "https://download.file-hunter.com" + parts[1].Trim().Replace("\\", "/").Substring(1, parts[1].Trim().Length - 1)
                                });
                            }
                        }

                    }
                    if (filename == "Update-Log.txt")
                    {
                        updatelistado = File.ReadAllText("Update-Log.txt");
                        File.Delete(filename);
                        string[] lines = updatelistado.Trim().Split('\n');

                        // Crear una lista para almacenar objetos
                        fileListUpdate = new List<FileData>();
                        foreach (var line in lines)
                        {
                            // Separar los elementos por tabulación
                            string[] parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);

                            // Crear un objeto FileData si la línea tiene al menos 3 partes
                            if (parts.Length >= 3)
                            {
                                fileListUpdate.Add(new FileData
                                {
                                    Date = parts[0].Trim(),
                                    Status = parts[1].Trim(),
                                    Url = parts[2].Trim()
                                });
                            }
                        }
                    }
                    if (ContadorDescargas == 2)
                    {
                        toolStripStatusLabel1.Text = "Añadiendo datos al grid...por favor espere";
                        Task.Run(() =>
                         {
                             Invoke((Action)(() =>
                             {
                                 // Asignar los datos al DataGridView en el hilo principal
                                 BindDataToDataGridView(fileList, dataGridView1);
                                 BindDataToDataGridView(fileListUpdate, dataGridView2);
                                 SetupTextBoxFilter();

                             }));
                         });


                    }

                }

            };
            return new AsyncCompletedEventHandler(action);

        }
        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            toolStripStatusLabel1.Text = "Descargando..." + e.ProgressPercentage + "% de ( " + e.BytesReceived + " / " + e.TotalBytesToReceive + ")";
        }
        /// <summary>
        /// Agrega un nuevo elemento al ContextMenuStrip.
        /// </summary>
        /// <param name="contextMenuStrip">El ContextMenuStrip al cual agregar el elemento.</param>
        /// <param name="itemText">El texto del nuevo elemento.</param>
        /// <param name="position">La posición en la que insertar el elemento (0 para inicio).</param>
        /// <param name="isChecked">Si el elemento debe estar marcado inicialmente.</param>
        /// <returns>El elemento creado.</returns>
        static ToolStripMenuItem AddMenuItem(ContextMenuStrip contextMenuStrip, string itemText, int position, bool isChecked)
        {
            // Crear un nuevo ToolStripMenuItem
            ToolStripMenuItem newItem = new ToolStripMenuItem(itemText)
            {
                CheckOnClick = false,
                Checked = false
            };

            // Asociar un evento Click al ToolStripMenuItem
            newItem.Click += (sender, e) =>
            {
                ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
                if (clickedItem != null)
                {
                    // MessageBox.Show($"¡Hiciste clic en: {clickedItem.Text}!");
                }
            };

            // Validar la posición y agregar el elemento al menú
            if (position < 0 || position > contextMenuStrip.Items.Count)
            {
                position = contextMenuStrip.Items.Count; // Agregar al final si la posición es inválida
            }
            contextMenuStrip.Items.Insert(position, newItem);

            return newItem; // Retornar el elemento creado
        }
        /// <summary>
        /// Agrega un ComboBox como subitem de un elemento del menú.
        /// </summary>
        /// <param name="parentItem">El elemento principal al cual agregar el ComboBox.</param>
        /// <param name="items">Los elementos del ComboBox.</param>
        /// <param name="onSelectedIndexChanged">Acción a ejecutar cuando se selecciona un ítem.</param>
        static void AddComboBoxSubItem(string name, ToolStripMenuItem parentItem, string[] items, Action<string> onSelectedIndexChanged, bool AddSeparator)
        {
            // Crear un ComboBox
            System.Windows.Forms.ComboBox comboBox = new System.Windows.Forms.ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList // Solo permite seleccionar de la lista
            };
            comboBox.Name = name;
            // Agregar los elementos al ComboBox
            comboBox.Items.AddRange(items);
            if (items.Length > 0) { comboBox.SelectedIndex = 0; }
            // Manejar el evento SelectedIndexChanged
            comboBox.SelectedIndexChanged += (sender, e) =>
            {
                if (comboBox.SelectedItem != null)
                {
                    onSelectedIndexChanged?.Invoke(comboBox.SelectedItem.ToString());
                    // MessageBox.Show($"Seleccionaste: {comboBox.SelectedItem}");
                }
            };

            // Crear un ToolStripControlHost para el ComboBox
            ToolStripControlHost comboBoxHost = new ToolStripControlHost(comboBox);

            // Agregar el ComboBox como subitem al elemento principal
            parentItem.DropDownItems.Add(comboBoxHost);
            if (AddSeparator) parentItem.DropDownItems.Add(new ToolStripSeparator());
        }
        /// <summary>
        /// Agrega un subelemento (subitem) a un elemento existente del menú.
        /// </summary>
        /// <param name="parentItem">El elemento principal al cual agregar el subitem.</param>
        /// <param name="subItemText">El texto del subitem.</param>
        /// <returns>El subitem creado.</returns>
        static ToolStripMenuItem AddSubMenuItemToolStrip(ToolStripMenuItem parentItem, string subItemText, bool Addseparator, Action<bool> clickedItem)
        {
            // Crear un nuevo ToolStripMenuItem como subitem
            ToolStripMenuItem subItem = new ToolStripMenuItem(subItemText);
            // Asociar un evento Click al subitem
            subItem.Click += (sender, e) =>
            {
                ToolStripMenuItem clickedSubItem = sender as ToolStripMenuItem;
                if (clickedSubItem != null)
                {
                    // MessageBox.Show($"¡Hiciste clic en el subitem: {clickedSubItem.Text}!");
                    // if (clickedSubItem.Text == "Lanza OpenMSX") { MessageBox.Show(""); }
                    clickedItem?.Invoke(true);
                }
            };

            // Agregar el subitem al elemento principal
            parentItem.DropDownItems.Add(subItem);
            if (Addseparator) parentItem.DropDownItems.Add(new ToolStripSeparator());
            return subItem; // Retornar el subitem creado
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            //iniciacilzamos config...
            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //config.AppSettings.Settings.Add("NewSetting", "SomeValue");
            //config.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection("appSettings");
            //config.AppSettings.Settings["DescomprimirDespuesdeDescargar"].Value = "true";
            PathOpenMSX = isOpenMSXInstalled("openmsx");
            if (!Directory.Exists(Application.StartupPath + "\\tmp\\")) Directory.CreateDirectory(Application.StartupPath + "\\tmp\\");
            ClearDirectory(Application.StartupPath + "\\tmp\\");
            toolStripComboBox3.SelectedIndex = 0;
            if (PathOpenMSX is not null)
            {
                ToolStripMenuItem OpenMSX = AddMenuItem(contextMenuStrip1, "OpenMSX", 4, false);
                //AddSubMenuItemToolStrip(OpenMSX, "TIPO DE MAPPER", true, clickedItem =>
                //{
                //    //MessageBox.Show(selectedItem);
                //    // MessageBox.Show($"Seleccionaste: {selectedItem}");
                //});

                AddSubMenuItemToolStrip(OpenMSX, "Ejecuta Seleccionado", false, clickedItem =>
                {
                    if (IsControlVisible(dataGridView1))
                    {
                        int a = 0;
                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            a++;
                        }
                        if (a > 1) { MessageBox.Show("Seleccione solo un archivo, por favor.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            Uri uri = new Uri(row.Cells["FilePath"].Value.ToString());
                            string fileName = uri.Segments[uri.Segments.Length - 1];
                            // archivosdescargados = "Descargado ";
                            TaskDownloadFileArchivos(Application.StartupPath + "\\tmp\\" + fileName.Replace("%20", "_"), uri.ToString(), true);

                        }
                    }
                    if (IsControlVisible(dataGridView2))
                    {
                        int a = 0;
                        foreach (DataGridViewRow row in dataGridView2.SelectedRows)
                        {
                            a++;
                        }
                        if (a > 1) { MessageBox.Show("Seleccione solo un archivo, por favor.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                        foreach (DataGridViewRow row in dataGridView2.SelectedRows)
                        {
                            Uri uri = new Uri(row.Cells["Url"].Value.ToString());
                            string fileName = uri.Segments[uri.Segments.Length - 1];
                            // archivosdescargados = "Descargado ";
                            TaskDownloadFileArchivos(Application.StartupPath + "\\tmp\\" + fileName.Replace("%20", "_"), uri.ToString(), true);

                        }
                    }
                });
                AddComboBoxSubItem("Mappers", OpenMSX, new string[] { "Mapper Auto", "Konami 5", "Konami 4", "ASCII 16", "ASCII 8" }
                , selectedItem =>
                {
                    TipoDeMApper = selectedItem;
                    MessageBox.Show("Se forzará en OpenMSX el tipo de mapper " + selectedItem, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // MessageBox.Show($"Seleccionaste: {selectedItem}");
                }, true);
                AddSubMenuItemToolStrip(OpenMSX, "Ejecuta ROM/DSK/CAS", false, clickedItem =>
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filter = "Archivos ROM/DSK/CAS (*.rom;*.dsk;*.cas)|*.rom;*.dsk;*.cas|Todos los archivos (*.*)|*.*";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            string arguments2 = "";
                            if (TipoDeMApper == "Mapper Auto")
                                if (TipoDeMApper == "Konami 5") arguments2 = " -romtype konami5";
                            if (TipoDeMApper == "Konami 4") arguments2 = " -romtype konami4";
                            if (TipoDeMApper == "ASCII 16") arguments2 = " -romtype ASCII16";
                            if (TipoDeMApper == "ASCII 8") arguments2 = " -romtype ASCII8";
                            ProcessStartInfo psi = new ProcessStartInfo();
                            psi.FileName = PathOpenMSX + "\\openmsx.exe";
                            if (dlg.FileName.ToLower().EndsWith(".dsk")) psi.Arguments = " \"" + dlg.FileName + "\"";
                            if (dlg.FileName.ToLower().EndsWith(".rom")) psi.Arguments = " -cart \"" + dlg.FileName + "\"" + arguments2;
                            if (dlg.FileName.ToLower().EndsWith(".cas")) psi.Arguments = " \"" + dlg.FileName + "\"";
                            psi.CreateNoWindow = true;
                            psi.WorkingDirectory = PathOpenMSX;
                            var p = Process.Start(psi);
                            this.WindowState = FormWindowState.Minimized;
                            p.WaitForExit();
                            //borraremos el contenido de tmp
                            ClearDirectory(Application.StartupPath + "\\tmp\\");
                            this.WindowState = FormWindowState.Normal;
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Error lanzando " + dlg.FileName, "Error");
                        }
                    }
                });
            }
            descomprimirDespuesDeDescargarToolStripMenuItem.Checked = bool.Parse(ConfigurationManager.AppSettings["DescomprimirDespuesdeDescargar"]);


            //listamos los puertos com si existen en el equipo si no devolvera la palabra null
            string[] comPorts = SerialPort.GetPortNames();

            // Iterar sobre los puertos COM utilizando foreach
            if (comPorts.Length > 0)
            {
                foreach (string port in comPorts)
                {
                    // Mostrar cada puerto COM disponible
                    // MessageBox.Show($"Puerto COM encontrado: {port}");
                    toolStripComboBox1.Items.Add(port);
                }
            }
            if (toolStripComboBox1.Items.Count == 0) { toolStripComboBox1.Items.Add("SIn Puertos Com"); } else { toolStripComboBox1.SelectedIndex = 0; }
            var file1DownloadTask1 = TaskDownloadFile("sha1sums.txt", "https://download.file-hunter.com/sha1sums.txt");
            var file1DownloadTask2 = TaskDownloadFile("Update-Log.txt", "https://download.file-hunter.com/Update-Log.txt");

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)

            {
                // buscaremos
                // ApplyFilter(textBox1.Text);
                panel1.Visible = false;
                buscarToolStripMenuItem1.Checked = false;
                textBox1.Text = string.Empty;
            }
        }


        private void BindDataToDataGridView(List<FileData> _data, DataGridView _dvControl)
        {
            // IEnumerable<FileData> data = _data;
            if (_dvControl == dataGridView1) _dataTableDV1 = new DataTable();
            if (_dvControl == dataGridView1) _dataTableDV2 = new DataTable();

            using (var reader = ObjectReader.Create(_data))
            {
                if (_dvControl == dataGridView1) _dataTableDV1.Load(reader);
                if (_dvControl == dataGridView2) _dataTableDV2.Load(reader);
            }
            if (_dvControl == dataGridView1) _dvControl.DataSource = _dataTableDV1;
            if (_dvControl == dataGridView2) _dvControl.DataSource = _dataTableDV2;
            ConfigureDataGridViewAppearance(_dvControl);
            foreach (DataGridViewColumn column in _dvControl.Columns)
            {
                if (_dvControl == dataGridView1) { if (!column.Name.Contains("FilePath")) column.Visible = false; }
                if (_dvControl == dataGridView2) { if (column.Name.Contains("FilePath")) column.Visible = false; }
            }
            _dvControl.Columns["Hash"].Visible = false;
            toolStripStatusLabel1.Text = "Total de archivos: " + _dvControl.RowCount.ToString();
            //seleccionamos el primer row
            if (_dvControl.Rows.Count > 0)
            {
                // Establecer la celda activa (seleccionada) en la primera fila, primera columna
                _dvControl.Rows[0].Selected = true;
            }
        }
        private void SetupTextBoxFilter()
        {
            // Configurar evento de filtrado en tiempo real
            if (IsControlVisible(dataGridView1))
            {
                textBox1.TextChanged += (sender, e) => ApplyFilter(textBox1.Text, _dataTableDV1);
                label1.Visible = true;
                checkBox1.Visible = false;
                checkBox2.Visible = false;
                checkBox3.Visible = false;
            }
            if (IsControlVisible(dataGridView2))
            {
                textBox1.TextChanged += (sender, e) => ApplyFilter(textBox1.Text, _dataTableDV2);
                label1.Visible = false;
                checkBox1.Visible = true;
                checkBox2.Visible = true;
                checkBox3.Visible = true;
            }
        }

        private void ApplyFilter(string filterText, DataTable _dataTable)
        {
            string columnaAbuscar = "";
            if (_dataTable == null) return;
            if (IsControlVisible(dataGridView1)) columnaAbuscar = "FilePath";
            if (IsControlVisible(dataGridView2))
            {
                if (checkBox1.Checked) columnaAbuscar = "Date";
                if (checkBox2.Checked) columnaAbuscar = "Status";
                if (checkBox3.Checked) columnaAbuscar = "Url";

            }
            string filterExpression = string.IsNullOrWhiteSpace(filterText)
        ? string.Empty
         : $"{columnaAbuscar} LIKE '%{filterText.Replace("'", "''")}%'";

            (_dataTable.DefaultView).RowFilter = filterExpression;
            toolStripStatusLabel1.Text = "Total de archivos: " + (_dataTable.DefaultView).Count.ToString();
        }

        public bool IsControlVisible(Control control)
        {
            if (!control.Visible)
                return false;

            Control parent = control.Parent;
            while (parent != null)
            {
                if (!parent.Visible)
                    return false;

                parent = parent.Parent;
            }

            Rectangle controlBounds = control.RectangleToScreen(control.ClientRectangle);
            Screen screen = Screen.FromControl(control);
            return screen.WorkingArea.IntersectsWith(controlBounds);
        }


        private void ConfigureDataGridViewAppearance(DataGridView _dgView)
        {
            _dgView.SuspendLayout();

            // General
            _dgView.BorderStyle = BorderStyle.Fixed3D;
            _dgView.BackgroundColor = Color.Beige; // Light beige background for the grid
            _dgView.EnableHeadersVisualStyles = false;

            // Estilo del encabezado (Header Style)
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.BackColor = Color.SaddleBrown; // Deep brown for header
            headerStyle.ForeColor = Color.White; // White text for contrast
            headerStyle.Font = new Font("Arial", 10F, FontStyle.Bold);
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _dgView.ColumnHeadersDefaultCellStyle = headerStyle;

            // Estilo de filas (Row Style)
            DataGridViewCellStyle rowStyle = new DataGridViewCellStyle();
            rowStyle.BackColor = Color.Wheat; // Light brown for rows
            rowStyle.ForeColor = Color.Black; // Dark text for contrast
            rowStyle.Font = new Font("Arial", 9F, FontStyle.Regular);
            rowStyle.SelectionBackColor = Color.Peru; // Darker brown when selecting
            rowStyle.SelectionForeColor = Color.White; // White text when selecting
            _dgView.DefaultCellStyle = rowStyle;

            // Estilo de filas alternadas (Alternating Row Style)
            DataGridViewCellStyle alternatingRowStyle = new DataGridViewCellStyle();
            alternatingRowStyle.BackColor = Color.Moccasin; // Lighter brown for alternating rows
            alternatingRowStyle.ForeColor = Color.Black;
            _dgView.AlternatingRowsDefaultCellStyle = alternatingRowStyle;

            // Bordes y selección (Borders and Selection)
            _dgView.GridColor = Color.DarkGoldenrod; // Golden-brown grid lines
            _dgView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _dgView.RowHeadersVisible = false; // Hide row header (optional)

            // Ajuste del ancho de las columnas (Column Width Adjustment)
            _dgView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Permitir la selección completa de la fila (Allow full row selection)
            _dgView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgView.MultiSelect = true; // Allow multi-selection
            _dgView.ResumeLayout();
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //    pictureBox1.Visible = false;
            //    pictureBox1.Image = null;
            //    Uri uri = new Uri(dataGridView1.CurrentCell.Value.ToString());
            //    // Extraer el nombre del archivo desde la ruta de la URL
            //    string fileName = uri.Segments[uri.Segments.Length - 1];
            //    string html = GetHtmlCode(fileName);
            //    List<string> urls = GetUrls(html);
            //    foreach (string url in urls)
            //    {
            //        byte[] image = GetImage(url);
            //        if (image != null)
            //        {
            //            using (var ms = new MemoryStream(image))
            //            {
            //                pictureBox1.Image = Image.FromStream(ms);
            //            }
            //            this.Text = url;
            //            break;
            //        }
            //       // MessageBox.Show(url); 
            //    }
            //   if (pictureBox1.Image !=null) pictureBox1.Visible = true;
            ////string[] imagenes = GetImageUrls(fileName);
            ////foreach (string imagen in imagenes)
            ////{
            ////    MessageBox.Show(imagen);
            ////}
        }

        private void verNovedadesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (verNovedadesToolStripMenuItem.Checked)
            {
                textBox1.Text = "";
                dataGridView1.Visible = false;
                buscarToolStripMenuItem1.Checked = false;
                dataGridView2.Columns[0].Width = 90;
                dataGridView2.Columns[3].Width = 80;
                dataGridView2.BringToFront();
                dataGridView2.Visible = true;
            }
            else
            {
                dataGridView2.SendToBack();
                dataGridView2.Visible = false;
                dataGridView1.Visible = true;
            }
        }

        private void buscarToolStripMenuItem1_CheckedChanged(object sender, EventArgs e)
        {

            if (buscarToolStripMenuItem1.Checked)
            {
                SetupTextBoxFilter();
                panel1.BringToFront();
                panel1.Visible = true;
                panel1.Focus();
                textBox1.Focus();
            }
            else
            {
                panel1.SendToBack();
                panel1.Visible = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            checkBox2.Checked = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            checkBox3.Checked = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            checkBox3.Checked = false;
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = true;
            textBox1.Text = "";
            SetupTextBoxFilter();
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = true;
            textBox1.Text = "";
            SetupTextBoxFilter();
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            checkBox3.Checked = true;
            textBox1.Text = "";
            SetupTextBoxFilter();
        }

        private void descargarSeleccionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string paz = "";
            if (IsControlVisible(dataGridView1))
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                    {
                        folderDialog.Description = "Seleccione la carpeta donde se descargará el archivo:";
                        folderDialog.ShowNewFolderButton = true;

                        // Mostrar el cuadro de diálogo
                        if (folderDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Mostrar la ruta seleccionada en el cuadro de texto
                            paz = folderDialog.SelectedPath;
                        }
                    }
                    Uri uri = new Uri(row.Cells["FilePath"].Value.ToString());
                    string fileName = uri.Segments[uri.Segments.Length - 1];
                    archivosdescargados = "Descargado ";
                    TaskDownloadFileArchivos(paz + "\\" + fileName.Replace("%20", "_"), uri.ToString(), false);
                }
            }
            if (IsControlVisible(dataGridView2))
            {
                foreach (DataGridViewRow row in dataGridView2.SelectedRows)
                {
                    using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                    {
                        folderDialog.Description = "Seleccione la carpeta donde se descargará el archivo:";
                        folderDialog.ShowNewFolderButton = true;

                        // Mostrar el cuadro de diálogo
                        if (folderDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Mostrar la ruta seleccionada en el cuadro de texto
                            paz = folderDialog.SelectedPath;
                        }
                    }
                    Uri uri = new Uri(row.Cells["Url"].Value.ToString());
                    string fileName = uri.Segments[uri.Segments.Length - 1];
                    archivosdescargados = "Descargado ";
                    TaskDownloadFileArchivos(paz + "\\" + fileName.Replace("%20", "_"), uri.ToString(), false);
                }
            }

        }
        private async Task TaskDownloadFileArchivos(string Filename, string Uri, bool runInOpenMSX)
        {

            WebClient client = new WebClient();



           // client.DownloadFileCompleted += DownloadFileCompletedArchivos(Filename, runInOpenMSX);


            var eventHandler = new AsyncCompletedEventHandler(DownloadFileCompletedArchivos(Filename, runInOpenMSX));
           // eventHandler.Invoke(null, new AsyncCompletedEventArgs(null, false, new Exception("404: Not Found")));
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) => DownloadProgressCallbackArchivos(Filename, sender, e));
            await client.DownloadFileTaskAsync(Uri, Filename);



        }
        /// <summary>
        /// Busca archivos recursivamente en un directorio y sus subdirectorios con extensiones específicas.
        /// </summary>
        /// <param name="path">Ruta del directorio inicial.</param>
        /// <param name="extensions">Array de extensiones a buscar (con punto, e.g., ".rom").</param>
        /// <returns>Lista de rutas completas de los archivos encontrados.</returns>
        static List<string> FindFilesRecursively(string path, string[] extensions)
        {
            List<string> foundFiles = new List<string>();

            try
            {
                // Buscar archivos en el directorio actual que coincidan con las extensiones
                foreach (var file in Directory.GetFiles(path))
                {
                    string extension = Path.GetExtension(file).ToLowerInvariant();
                    if (Array.Exists(extensions, ext => ext == extension))
                    {
                        foundFiles.Add(file);
                    }
                }

                // Buscar recursivamente en los subdirectorios
                foreach (var directory in Directory.GetDirectories(path))
                {
                    foundFiles.AddRange(FindFilesRecursively(directory, extensions));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"No se puede acceder al directorio: {path}. Detalles: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando el directorio: {path}. Detalles: {ex.Message}");
            }

            return foundFiles;
        }
        /// <summary>
        /// Muestra un cuadro de diálogo preguntando si se debe ejecutar un archivo.
        /// </summary>
        /// <param name="filePath">Ruta completa del archivo a ejecutar.</param>
        /// <returns>True si el usuario selecciona 'Sí', False si selecciona 'No'.</returns>
        static bool AskToExecuteFile(string filePath)
        {
            // Validar que la ruta no sea nula o vacía
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("La ruta del archivo no puede ser nula o vacía.", nameof(filePath));

            // Construir el mensaje para el cuadro de diálogo
            string message = $"¿Deseas ejecutar el archivo?\n{filePath}";
            string caption = "Confirmación de ejecución";

            // Mostrar el cuadro de diálogo con botones Sí y No
            DialogResult result = MessageBox.Show(
                message,                  // Texto del cuadro de diálogo
                caption,                  // Título del cuadro de diálogo
                MessageBoxButtons.YesNo,  // Botones disponibles
                MessageBoxIcon.Question   // Icono del cuadro de diálogo
            );

            // Retornar True si el usuario selecciona 'Sí', False si selecciona 'No'
            return result == DialogResult.Yes;
        }
        public AsyncCompletedEventHandler DownloadFileCompletedArchivos(string filename, bool runInOpenMSX)
        {

                Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
                {

                    var _filename = filename;
                    if (e.Error != null)
                    {

                        throw e.Error;
                    }
                    else
                    {
                            if (runInOpenMSX)
                            {
                                if (filename.ToLower().EndsWith(".zip"))
                                {
                                    string nombre = System.IO.Path.GetFileName(filename);
                                    string path = System.IO.Path.GetDirectoryName(filename);
                                    ZipFile.ExtractToDirectory(filename, Application.StartupPath + "\\tmp\\", true);
                                    File.Delete(filename);
                                    var files = FindFilesRecursively(Application.StartupPath + "\\tmp\\", new[] { ".rom", ".dsk", ".cas" });
                                    foreach (var file in files)
                                    {
                                        int a = 0;
                                        foreach (var filee in files)
                                        {
                                            a++;
                                        }
                                        if (a > 1)
                                        {
                                            if (AskToExecuteFile(file))
                                            {
                                                string arguments2 = "";
                                                if (TipoDeMApper == "Mapper Auto")
                                                    if (TipoDeMApper == "Konami 5") arguments2 = " -romtype konami5";
                                                if (TipoDeMApper == "Konami 4") arguments2 = " -romtype konami4";
                                                if (TipoDeMApper == "ASCII 16") arguments2 = " -romtype ASCII16";
                                                if (TipoDeMApper == "ASCII 8") arguments2 = " -romtype ASCII8";
                                                ProcessStartInfo psi = new ProcessStartInfo();
                                                psi.FileName = PathOpenMSX + "\\openmsx.exe";
                                                if (file.ToLower().EndsWith(".dsk")) psi.Arguments = " \"" + file + "\"";
                                                if (file.ToLower().EndsWith(".rom")) psi.Arguments = " -cart \"" + file + "\"" + arguments2;
                                                if (file.ToLower().EndsWith(".cas")) psi.Arguments = " \"" + file + "\"";
                                                psi.CreateNoWindow = true;
                                                psi.WorkingDirectory = PathOpenMSX;
                                                var p = Process.Start(psi);
                                                this.WindowState = FormWindowState.Minimized;
                                                p.WaitForExit();
                                                //borraremos el contenido de tmp
                                                ClearDirectory(Application.StartupPath + "\\tmp\\");
                                                // Restaurar la ventana al hacer doble clic en el ícono
                                                this.Show();
                                                this.WindowState = FormWindowState.Normal;
                                                notifyIcon.Visible = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            string arguments2 = "";
                                            if (TipoDeMApper == "Auto")
                                                if (TipoDeMApper == "Konami 5") arguments2 = " -romtype konami5";
                                            if (TipoDeMApper == "Konami 4") arguments2 = " -romtype konami4";
                                            if (TipoDeMApper == "ASCII 16") arguments2 = " -romtype ASCII16";
                                            if (TipoDeMApper == "ASCII 8") arguments2 = " -romtype ASCII8";
                                            ProcessStartInfo psi = new ProcessStartInfo();
                                            psi.FileName = PathOpenMSX + "\\openmsx.exe";
                                            if (file.ToLower().EndsWith(".dsk")) psi.Arguments = " \"" + file + "\"";
                                            if (file.ToLower().EndsWith(".rom")) psi.Arguments = " -cart \"" + file + "\"" + arguments2;
                                            if (file.ToLower().EndsWith(".cas")) psi.Arguments = " \"" + file + "\"";
                                            psi.CreateNoWindow = true;
                                            psi.WorkingDirectory = PathOpenMSX;
                                            var p = Process.Start(psi);
                                            this.WindowState = FormWindowState.Minimized;
                                            p.WaitForExit();
                                            //borraremos el contenido de tmp
                                            ClearDirectory(Application.StartupPath + "\\tmp\\");
                                            // Restaurar la ventana al hacer doble clic en el ícono
                                            this.Show();
                                            this.WindowState = FormWindowState.Normal;
                                            notifyIcon.Visible = false;
                                            break;
                                        }
                                    }

                                }
                                if (filename.ToLower().EndsWith(".rom") || filename.ToLower().EndsWith(".cas") || filename.ToLower().EndsWith(".dsk"))
                                {
                                    string arguments2 = "";
                                    if (TipoDeMApper == "Auto")
                                        if (TipoDeMApper == "Konami 5") arguments2 = " -romtype konami5";
                                    if (TipoDeMApper == "Konami 4") arguments2 = " -romtype konami4";
                                    if (TipoDeMApper == "ASCII 16") arguments2 = " -romtype ASCII16";
                                    if (TipoDeMApper == "ASCII 8") arguments2 = " -romtype ASCII8";
                                    ProcessStartInfo psi = new ProcessStartInfo();
                                    psi.FileName = PathOpenMSX + "\\openmsx.exe";
                                    if (filename.ToLower().EndsWith(".dsk")) psi.Arguments = " \"" + filename + "\"";
                                    if (filename.ToLower().EndsWith(".rom")) psi.Arguments = " -cart \"" + filename + "\"" + arguments2;
                                    if (filename.ToLower().EndsWith(".cas")) psi.Arguments = " \"" + filename + "\"";
                                    psi.CreateNoWindow = true;
                                    psi.WorkingDirectory = PathOpenMSX;
                                    var p = Process.Start(psi);
                                    this.WindowState = FormWindowState.Minimized;
                                    p.WaitForExit();
                                    //borraremos el contenido de tmp
                                    ClearDirectory(Application.StartupPath + "\\tmp\\");
                                    // Restaurar la ventana al hacer doble clic en el ícono
                                    this.Show();
                                    this.WindowState = FormWindowState.Normal;
                                    notifyIcon.Visible = false;

                                }
                                //Hay que tratar el resto de archivos?¿?¿
                            }
                            else
                            //comprobar en caso de ser zip si hay que descomprimir
                            //....todo
                            if (descomprimirDespuesDeDescargarToolStripMenuItem.Checked)
                            {
                                if (filename.ToLower().EndsWith(".zip"))
                                {
                                    string nombre = System.IO.Path.GetFileName(filename);
                                    string path = System.IO.Path.GetDirectoryName(filename);
                                    ZipFile.ExtractToDirectory(filename, path + "\\" + nombre.Replace(".zip", ""));
                                    File.Delete(filename);
                                }
                            }
                            archivosdescargados += filename + " ";
                            toolStripStatusLabel1.Text = archivosdescargados;
                       
                    }


                };
           
            return new AsyncCompletedEventHandler(action);
            
            //catch (WebException ex)
            //{
            //    MessageBox.Show("Ocurrio un error al descargar el archivo:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    throw;
            //}
            //catch (InvalidOperationException)
            //{
            //    MessageBox.Show("Ocurrio un error al guardar el archivo:", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Ocurrio un error al descargar el archivo:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //return null;
        }
        /// <summary>
        /// Borra el contenido de un directorio, incluyendo archivos y subdirectorios.
        /// </summary>
        /// <param name="directoryPath">Ruta del directorio cuyo contenido se debe borrar.</param>
        static void ClearDirectory(string directoryPath)
        {
            // Validar que el directorio exista
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
                throw new ArgumentException("La ruta del directorio no es válida o no existe.", nameof(directoryPath));

            // Borrar todos los archivos del directorio
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                File.Delete(file);
            }

            // Borrar todos los subdirectorios y su contenido
            foreach (var directory in Directory.GetDirectories(directoryPath))
            {
                Directory.Delete(directory, true); // El segundo argumento `true` borra recursivamente
            }
        }
        private void DownloadProgressCallbackArchivos(string nombreArchivo, object sender, DownloadProgressChangedEventArgs e)
        {
            // Ejemplo: mostrar progreso y nombre de archivo
            toolStripStatusLabel1.Text = "Descargando " + nombreArchivo + " " + e.ProgressPercentage + "% de ( " + e.BytesReceived + " / " + e.TotalBytesToReceive + ")";
        }
        private readonly static string reservedCharacters = "!*'();:@&=+$,?%#[]";
        public static string UrlEncode(string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            var sb = new StringBuilder();

            foreach (char @char in value)
            {
                if (reservedCharacters.IndexOf(@char) == -1)
                    sb.Append(@char);
                else
                    sb.AppendFormat("%{0:X2}", (int)@char);
            }
            return sb.ToString();
        }
        private void webMSXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow is not null)
            {
                string url = "";
                if (IsControlVisible(dataGridView1)) url = UrlEncode(dataGridView1.CurrentRow.Cells["FilePath"].Value.ToString());
                if (IsControlVisible(dataGridView2)) url = dataGridView2.CurrentRow.Cells["Url"].Value.ToString();
                //no multiple seleccion
                int a = 0;
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    a++;
                }
                if (a > 1) { MessageBox.Show("Seleccione solo un elemento de la lista, por favor.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                Process.Start(new ProcessStartInfo("https://download.file-hunter.com/assets/webmsx.html?url=" + url) { UseShellExecute = true });
            }
        }

        private void descomprimirDespuesDeDescargarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (descomprimirDespuesDeDescargarToolStripMenuItem.Checked)
            {
                config.AppSettings.Settings["DescomprimirDespuesdeDescargar"].Value = "true";
            }
            else
            {
                config.AppSettings.Settings["DescomprimirDespuesdeDescargar"].Value = "false";
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (IsControlVisible(dataGridView2) && dataGridView2.CurrentRow is not null)
            {
                Uri url = new Uri(dataGridView2.CurrentRow.Cells["Url"].Value.ToString());
                toolStripStatusLabel1.Text = "Total de archivos: " + dataGridView2.RowCount.ToString() + " --> " + WebUtility.UrlDecode(url.Segments.Last().TrimEnd('/'));
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (IsControlVisible(dataGridView1) && dataGridView1.CurrentRow is not null)
            {
                Uri url = new Uri(dataGridView1.CurrentRow.Cells["FilePath"].Value.ToString());
                toolStripStatusLabel1.Text = "Total de archivos: " + dataGridView1.RowCount.ToString() + " --> " + WebUtility.UrlDecode(url.Segments.Last().TrimEnd('/'));
            }
        }

        private void toolStripComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox3.SelectedItem.ToString() != "Filtro rapido") ApplyFilter(toolStripComboBox3.SelectedItem.ToString(), _dataTableDV1);
        }

        private void fjntr_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private static void ExtractEmbeddedResource(string resourceName, string outputPath)
        {
            using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (resourceStream == null)
                throw new Exception($"Recurso '{resourceName}' no encontrado.");

            using FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            resourceStream.CopyTo(fileStream);
        }


        private void dSKROMToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string outputPath = Application.StartupPath + "\\tmp\\dsk2rom.exe";
            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.ToLower().Contains("dsk2rom.exe"))
                {
                   ExtractEmbeddedResource(resourceName, outputPath);   
                }
                   
            }
            Dsk2RomFrm d2r = new Dsk2RomFrm();
            d2r.Show();
        }
    }
}


public class FileData
{
    public string Hash { get; set; }
    public string FilePath { get; set; }
    public string Date { get; set; }
    public string Status { get; set; }
    public string Url { get; set; }
}

internal class SectorData
{
    public byte[] Data { get; set; }
    public int Size { get; set; }
}