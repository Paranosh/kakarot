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
using Konamiman.JoySerTrans;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Policy;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Net.Http.Headers;


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
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            InitializeClipboardMonitor();
        }
        static long fileLength;
        static long bytesSent;
        static string filenameSent;
        ToolStripMenuItem OpenMSX, Launcher, Favoritos;
        WebBrowser webBrowser = new WebBrowser();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string PathOpenMSX, TipoDeMApper, acumulador = "";
        private System.Windows.Forms.Timer fadeTimer;
        private float opacityIncrement;
        private bool fadeIn, permitirVariasInstancias, descargando = false;
        private DataTable _dataTableDV1;
        private DataTable _dataTableDV2;
        string listado = "", updatelistado = "", archivosdescargados = "Descargado ";
        List<FileData> fileList, fileListUpdate;
        List<string> originalItems = new List<string>();
        private System.Windows.Forms.Timer clipboardMonitorTimer;
        private string lastClipboardText = string.Empty;
        int ContadorDescargas = 0;
        string memoriarutas = "", nivelAnterior = "", nivelAnteriorPre = "", nivelActual = "", repoenuso="";
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
            // Asegurar que el panel de busqueda esté siempre en la esquina inferior derecha
            panel1.Left = this.ClientSize.Width - panel1.Width - 50;  // 10px de margen de la derecha
            panel1.Top = this.ClientSize.Height - panel1.Height - 50; // 10px de margen inferior
            listBox1.Width = this.ClientSize.Width;
            listBox1.Height = this.ClientSize.Height - 20;
            listBox2.Width = this.ClientSize.Width;
            listBox2.Height = this.ClientSize.Height - 20;
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
                        int b = 0;
                        string UltimoArchivoActualizado = "";
                        updatelistado = File.ReadAllText("Update-Log.txt");
                        File.Delete(filename);
                        string[] lines = updatelistado.Trim().Split('\n');
                        // Crear una lista para almacenar objetos
                        fileListUpdate = new List<FileData>();
                        //el primer item es el archivo mas nuevo en fj
                        foreach (var line in lines)
                        {
                            // Separar los elementos por tabulación
                            string[] parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                            // Crear un objeto FileData si la línea tiene al menos 3 partes
                            if (b == 0) UltimoArchivoActualizado = parts[2].Trim();
                            if (parts.Length >= 3)
                            {
                                fileListUpdate.Add(new FileData
                                {
                                    Date = parts[0].Trim(),
                                    Status = parts[1].Trim(),
                                    Url = parts[2].Trim()

                                });
                            }
                            b++;
                        }
                        if (bool.Parse(ConfigurationManager.AppSettings["InformaDeActualizacionesFileHunter"]))
                        {
                            informarDeActualizacionesToolStripMenuItem.Checked = true;
                            var ultimovisto = ConfigurationManager.AppSettings["UltimoCheckNovedadesFileHunter"].ToString();
                            if (ultimovisto != UltimoArchivoActualizado)
                            {
                                var userResult = AutoClosingMessageBox.Show("Hay novedades en File-hunter, ¿Deseas verlas?", "Updates", 500, MessageBoxButtons.YesNo);
                                if (userResult == System.Windows.Forms.DialogResult.Yes)
                                {
                                    verNovedadesToolStripMenuItem.PerformClick();
                                }
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
        static ToolStripMenuItem AddMenuItem(ContextMenuStrip contextMenuStrip, string itemText, int position, bool isChecked, string Name)
        {
            // Crear un nuevo ToolStripMenuItem
            ToolStripMenuItem newItem = new ToolStripMenuItem(itemText)
            {
                CheckOnClick = false,
                Checked = false
            };
            newItem.Name = Name;
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
        static ToolStripMenuItem AddSubMenuItemToolStrip(ToolStripMenuItem parentItem, string subItemText, bool Addseparator, Action<bool> clickedItem, string Name)
        {

                // Crear un nuevo ToolStripMenuItem como subitem
                ToolStripMenuItem subItem = new ToolStripMenuItem(subItemText);
                subItem.Name = Name;
                // Asociar un evento Click al subitem
                subItem.Click += (sender, e) =>
                {
                    ToolStripMenuItem clickedSubItem = sender as ToolStripMenuItem;
                    if (clickedSubItem != null)
                    {
                        foreach (object item in parentItem.DropDownItems) 
                        {
                            ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                            if (menuItem == null) { continue; }
                            menuItem.Checked = false; 
                        }
                        subItem.Checked = true;
                        clickedItem?.Invoke(true);
                    }
                };

                // Agregar el subitem al elemento principal
                parentItem.DropDownItems.Add(subItem);
                if (Addseparator) parentItem.DropDownItems.Add(new ToolStripSeparator());
                return subItem; // Retornar el subitem creado

        }
        static bool RemoveSubMenuItemToolStrip(ToolStripMenuItem parentItem, string subItemName)
        {
            // Buscar el subitem por su nombre
            foreach (ToolStripItem item in parentItem.DropDownItems)
            {
                if (item is ToolStripMenuItem subItem && subItem.Name == subItemName)
                {
                    // Eliminar el subitem encontrado
                    parentItem.DropDownItems.Remove(item);
                    return true; // Indicar que el subitem se eliminó con éxito
                }
            }
            return false; // Indicar que no se encontró el subitem
        }
        private void ApplyLanguage(string cultureCode)
        {
            try
            {
                // Cambia la cultura actual de la aplicación
                var culture = new System.Globalization.CultureInfo(cultureCode);
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

                // Carga los recursos del formulario
                var resources = new System.ComponentModel.ComponentResourceManager(this.GetType());

                // Aplica los recursos al formulario y a todos los controles
                ApplyResourcesToControls(this, resources);

                // Actualiza el texto del formulario mismo
                resources.ApplyResources(this, "$this");
            }
            catch (CultureNotFoundException)
            {
                MessageBox.Show($"Cultura no encontrada: {cultureCode}. Aplicando inglés por defecto.");
                ApplyLanguage("en");
            }
        }
        private void ApplyResourcesToControls(Control control, ComponentResourceManager resources)
        {
            // Aplica los recursos al control actual
            resources.ApplyResources(control, control.Name);

            // Aplica los recursos a los controles hijos (recursivamente)
            foreach (Control childControl in control.Controls)
            {
                ApplyResourcesToControls(childControl, resources);
            }
        }
        private void UpdateTexts()
        {
            // Actualiza los textos dinámicos usando Properties.Strings
            herramientasToolStripMenuItem.Text = kakarot.Strings.Herramientas;
            opcionesToolStripMenuItem.Text = kakarot.Strings.Opciones;
            buscarToolStripMenuItem1.Text = kakarot.Strings.Buscar;
            descargarSeleccionToolStripMenuItem.Text = kakarot.Strings.Descargar_seleccion;
            convertirAToolStripMenuItem.Text = kakarot.Strings.Convertir_a;
            aplicarParcheIPSToolStripMenuItem.Text = kakarot.Strings.AplicarParche;
            ConcatoolStripMenuItem.Text = kakarot.Strings.Concatenar;
            descomprimirDespuesDeDescargarToolStripMenuItem.Text = kakarot.Strings.DescomprimirDespuesDeDescargar;
            SetOpenMSXPathtoolStripMenuItem.Text = kakarot.Strings.EstablecerRutaParaOpenMSX;
            permitirMultiplesInstanciasToolStripMenuItem.Text = kakarot.Strings.PermitirMultiplesInstancias;
            IdiomaMenuItem.Text = kakarot.Strings.Idioma;
            verNovedadesToolStripMenuItem.Text = kakarot.Strings.VerNovedades;
            informarDeActualizacionesToolStripMenuItem.Text = kakarot.Strings.InformarDeActualizaciones;
            if (PathOpenMSX is not null)
            {
                if (OpenMSX is ToolStripMenuItem menuItem && menuItem.DropDownItems.Count > 0)
                {
                    foreach (ToolStripItem subItem in menuItem.DropDownItems)
                    {
                        if (subItem.Name == "EjecutaSelecionado") subItem.Text = kakarot.Strings.EjecutaSeleccionado;
                        if (subItem.Name == "EjecutaROMDSKCASWAV") subItem.Text = kakarot.Strings.EjecutaROM_DSK_CAS_WAV;
                    }
                }
            }
            toolStripComboBox3.Items[0] = kakarot.Strings.FiltroRapido;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser.Navigate("https://msxscans.file-hunter.com/");
            ConfigureListBoxAppearance(listBox1);
            Favoritos = AddMenuItem(contextMenuStrip1, "Favoritos", 9, false, "FavoritosMenu");
            if (File.Exists(Application.StartupPath + "\\tmp.ROM")) File.Delete(Application.StartupPath + "\\tmp.ROM");
            if (File.Exists(Application.StartupPath + "\\tmp.DSK")) File.Delete(Application.StartupPath + "\\tmp.DSK");
            if (!Directory.Exists(Application.StartupPath + "\\launcher")) Directory.CreateDirectory(Application.StartupPath + "\\launcher");
            if (!File.Exists(Application.StartupPath + "\\favoritos.fav"))
            {
                File.Create(Application.StartupPath + "\\favoritos.fav");
            }
            else
            {
                var Urls = File.ReadAllLines(Application.StartupPath + "\\favoritos.fav");
                foreach (var laUrl in Urls)
                {
                    AddSubMenuItemToolStrip(Favoritos, laUrl, false, clickedItem =>
                    {
                        Process.Start(new ProcessStartInfo(laUrl) { UseShellExecute = true });
                    }, laUrl);
                }
            }
            var ClavesAppConfig = ConfigurationManager.AppSettings;
            foreach (string _clave in ClavesAppConfig.AllKeys)
            {
                if (_clave.ToLower().Contains("githubrepo"))
                {
                    AddSubMenuItemToolStrip(reposToolStripMenuItem, _clave, false, clickedItem =>
                    {
                        Preparalistbox2(ClavesAppConfig.Get(_clave));
                    }, "lanza"+ ClavesAppConfig.Get(_clave));
                }
            }
            //iniciacilzamos config...
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //chuleta
            //config.AppSettings.Settings.Add("NewSetting", "SomeValue");
            //config.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection("appSettings");
            //config.AppSettings.Settings["DescomprimirDespuesdeDescargar"].Value = "true";
            if (config.AppSettings.Settings["OpenMSXPath"].Value == "")
            {
                PathOpenMSX = isOpenMSXInstalled("openmsx");
            }
            else
            {
                PathOpenMSX = config.AppSettings.Settings["OpenMSXPath"].Value;
            }
            if (config.AppSettings.Settings["Idioma"].Value == "en") { ApplyLanguage("en"); IdiomaEngtoolStripMenuItem.Checked = true; }
            if (config.AppSettings.Settings["Idioma"].Value == "es") { ApplyLanguage("es"); IdiomaSpatoolStripMenuItem.Checked = true; }
            UpdateTexts();
            if (!Directory.Exists(Application.StartupPath + "\\tmp\\")) Directory.CreateDirectory(Application.StartupPath + "\\tmp\\");
            ClearDirectory(Application.StartupPath + "\\tmp\\");
            toolStripComboBox3.SelectedIndex = 0;
            if (PathOpenMSX is not null)
            {
                /* config.AppSettings.Settings["OpenMSXPath"].Value = PathOpenMSX;
                 config.Save(ConfigurationSaveMode.Modified);
                 ConfigurationManager.RefreshSection("appSettings");*/
                OpenMSX = AddMenuItem(contextMenuStrip1, "OpenMSX", 4, false, "MenuOpenMSX");
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
                }, "EjecutaSelecionado");
                AddComboBoxSubItem("Mappers", OpenMSX, new string[] { "Mapper Auto", "Konami 5", "Konami 4", "ASCII 16", "ASCII 8" }
                , selectedItem =>
                {
                    TipoDeMApper = selectedItem;
                    MessageBox.Show("Se forzará en OpenMSX el tipo de mapper " + selectedItem, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }, true);
                AddSubMenuItemToolStrip(OpenMSX, "Ejecuta ROM/DSK/CAS/WAV", false, clickedItem =>
                {
                    var dlg = new OpenFileDialog();
                    dlg.Filter = "Archivos ROM/DSK/CAS (*.rom;*.dsk;*.cas;*.wav)|*.rom;*.dsk;*.cas;*.wav|Todos los archivos (*.*)|*.*";
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
                            if (dlg.FileName.ToLower().EndsWith(".wav")) psi.Arguments = " \"" + dlg.FileName + "\"";
                            psi.CreateNoWindow = true;
                            psi.WorkingDirectory = PathOpenMSX;
                            var p = Process.Start(psi);
                            this.WindowState = FormWindowState.Minimized;
                            p.WaitForExit();
                            //borraremos el contenido de tmp
                            ClearDirectory(Application.StartupPath + "\\tmp\\");
                            NotifyIcon_DoubleClick(sender, e);
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Error lanzando " + dlg.FileName, "Error");
                        }
                    }
                }, "EjecutaROMDSKCASWAV");
            }
            if (Directory.GetFiles(Application.StartupPath + "\\launcher").Length > 0)
            {
                Launcher = AddMenuItem(contextMenuStrip1, "Launcher", 9, false, "LaunchApps");
                foreach (string file in Directory.GetFiles(Application.StartupPath + "\\launcher"))
                {
                    string Shortfile = Path.GetFileNameWithoutExtension(file);
                    AddSubMenuItemToolStrip(Launcher, Shortfile, false, clickedItem =>
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.CreateNoWindow = true;
                        psi.UseShellExecute = false;
                        psi.FileName = file;
                        //   psi.Arguments = "-" + argumentos + argumentoCompresion + " tmp.DSK " + Application.StartupPath + "\\tmp.ROM";
                        var p = Process.Start(psi);
                        p.WaitForExit();
                    }, "EjecutaSelecionadoLauncher" + Shortfile);
                }
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
                    toolStripComboBox2.Items.Add(port);
                }
            }
            if (toolStripComboBox2.Items.Count == 0) { toolStripComboBox2.Items.Add("Sin Puertos Com"); } else { toolStripComboBox2.SelectedIndex = 0; }
            toolStripComboBox4.SelectedIndex = 1;
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
                label1.Visible = true;
                checkBox1.Visible = false;
                checkBox2.Visible = false;
                checkBox3.Visible = false;
            }
            if (IsControlVisible(dataGridView2))
            {
                label1.Visible = false;
                checkBox1.Visible = true;
                checkBox2.Visible = true;
                checkBox3.Visible = true;
            }

        }

        private void ApplyFilter(string filterText, object target, DataTable _dataTable = null)
        {
            try
            {
                if (target == null) return;

                if (target is ListBox listBox)
                {

                    // Aplicar el filtro
                    var filteredItems = string.IsNullOrWhiteSpace(filterText)
                        ? originalItems // Si no hay filtro, mostrar todos los elementos
                        : originalItems.Where(item => item.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                    // Actualizar los elementos del ListBox
                    listBox.BeginUpdate();
                    listBox.Items.Clear();
                    foreach (var item in filteredItems)
                    {
                        listBox.Items.Add(item);
                    }
                    listBox.EndUpdate();

                    // Actualizar el estado en un ToolStripStatusLabel si es necesario
                    toolStripStatusLabel1.Text = "Total de elementos: " + listBox.Items.Count;
                }
                else if (target is DataGridView dataGridView)
                {
                    // Manejar el filtrado en un DataTable asociado a un DataGridView
                    string columnaAbuscar = "";

                    if (_dataTable == null) return;

                    // Determinar la columna a buscar
                    if (IsControlVisible(dataGridView1))
                    {
                        columnaAbuscar = "FilePath";
                    }
                    else if (IsControlVisible(dataGridView2))
                    {
                        if (checkBox1.Checked) columnaAbuscar = "Date";
                        else if (checkBox2.Checked) columnaAbuscar = "Status";
                        else if (checkBox3.Checked) columnaAbuscar = "Url";
                    }

                    // Verificar si columnaAbuscar tiene un valor válido
                    if (string.IsNullOrEmpty(columnaAbuscar))
                    {
                        toolStripStatusLabel1.Text = "No se ha seleccionado una columna para aplicar el filtro.";
                        return;
                    }

                    // Construir la expresión de filtro
                    string filterExpression = string.IsNullOrWhiteSpace(filterText)
                        ? string.Empty
                        : $"{columnaAbuscar} LIKE '%{filterText.Replace("'", "''")}%'";

                    // Aplicar el filtro
                    _dataTable.DefaultView.RowFilter = filterExpression;

                    // Actualizar el estado en el ToolStripStatusLabel
                    toolStripStatusLabel1.Text = "Total de archivos: " + _dataTable.DefaultView.Count;
                }
                else
                {
                    throw new ArgumentException("El tipo de control no es compatible. Use ListBox o DataGridView.");
                }
            }
            catch (Exception ex) { MessageBox.Show("Ocurrio un error al aplicar el filtro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); textBox1.Text = ""; }
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
        private void ConfigureListBoxAppearance(ListBox listBox)
        {
            // Configuración general
            listBox.BorderStyle = BorderStyle.Fixed3D;
            listBox.BackColor = Color.DarkGray;//.Beige;
            listBox.ForeColor = Color.Black;
            listBox.Font = new Font("Arial", 10F, FontStyle.Regular);

            // Selección múltiple
            listBox.SelectionMode = SelectionMode.MultiExtended;

            // Habilitar dibujo personalizado
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.ItemHeight = 22; // Altura fija para cada elemento

            listBox.DrawItem += (sender, e) =>
            {
                if (e.Index < 0) return;

                // Determinar si el elemento está seleccionado
                bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

                // Colores de fondo y texto
                Color backgroundColor = isSelected ? Color.Peru : (e.Index % 2 == 0 ? Color.Wheat : Color.Moccasin);
                Color textColor = isSelected ? Color.White : Color.Black;

                // Dibujar fondo
                e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);

                // Calcular la posición del texto para centrarlo verticalmente
                string text = listBox.Items[e.Index].ToString();
                SizeF textSize = e.Graphics.MeasureString(text, e.Font);
                float textY = e.Bounds.Y + (e.Bounds.Height - textSize.Height) / 2;

                // Dibujar texto del elemento
                e.Graphics.DrawString(
                    text,
                    e.Font,
                    new SolidBrush(textColor),
                    new PointF(e.Bounds.X + 5, textY) // Texto alineado a la izquierda con un margen
                );

                // Dibujar línea negra horizontal (similar a las líneas de un DataGridView)
                using (Pen linePen = new Pen(Color.Black, 1))
                {
                    e.Graphics.DrawLine(linePen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                }

                e.DrawFocusRectangle();
            };
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
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                textBox1.Text = "";
                dataGridView1.Visible = false;
                buscarToolStripMenuItem1.Checked = false;
                dataGridView2.Columns[0].Width = 90;
                dataGridView2.Columns[3].Width = 80;
                dataGridView2.BringToFront();
                dataGridView2.Visible = true;
                config.AppSettings.Settings["UltimoCheckNovedadesFileHunter"].Value = dataGridView2.Rows[0].Cells[4].Value.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
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
                        //folderDialog.Description = "Seleccione la carpeta donde se descargará el archivo:";
                        //folderDialog.ShowNewFolderButton = true;

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
            if (IsControlVisible(listBox1))
            {
                if (listBox1.SelectedItem != null)
                {
                    listBox1_DoubleClick(sender, e);
                }
            }
            if (IsControlVisible(listBox2))
            {
                if (listBox2.SelectedItem.ToString().StartsWith("DIR:"))
                {
                    MessageBox.Show("No es posible descargar directorios", "Informacion",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                if (listBox2.SelectedItem.ToString().StartsWith("\\.") || listBox2.SelectedItem.ToString().StartsWith("\\..")) { return; }
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        toolStripStatusLabel1.Text = "-Descargando, por favor espere-";
                        string folder = fbd.SelectedPath;
                        // string NoeEncUrl = UrlEncode(listBox1.SelectedItem.ToString().Substring(2, listBox1.SelectedItem.ToString().Length - 2)).Replace("\\", "/").Replace(" ", "%20");
                        string url = listBox2.SelectedItem.ToString();
                        string decodedUrl = Uri.UnescapeDataString(listBox2.SelectedItem.ToString().Replace("DIR:", "").Replace("?ref=main", ""));
                        string fileName = decodedUrl.Substring(decodedUrl.LastIndexOf("/") + 1, (decodedUrl.Length - 1) - decodedUrl.LastIndexOf("/"));
                        if (url.EndsWith("yaml"))
                        {
                            url = yamlToURLDown(url).Trim();
                            fileName = url.Substring(url.LastIndexOf("/") + 1, (url.Length - 1) - url.LastIndexOf("/"));
                        }
                        string tempfile = Path.GetTempFileName();
                        string strPathnoFilename = Path.GetFileNameWithoutExtension(fileName);
                        string DestFolfer = folder + "\\" + strPathnoFilename + "\\";
                        if (Directory.Exists(DestFolfer))
                        {
                            DialogResult dialogResult = MessageBox.Show("El directorio de destino ya existe, si pulsa \"SI\" se borrará todo, incluyendo su contenido, ¿esta seguro?", "Advertencia", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                Directory.Delete(DestFolfer, true);

                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                MessageBox.Show("Cancelado");
                                return;

                            }
                        }
                        TaskDownloadFileArchivos(fbd.SelectedPath + "\\" + fileName, url, false);
                    }
                }
            }
        }
        private async Task TaskDownloadFileArchivos(string Filename, string Uri, bool runInOpenMSX)
        {
            WebClient client = new WebClient();
            client.DownloadFileCompleted += DownloadFileCompletedArchivos(Filename, runInOpenMSX);
            //var eventHandler = new AsyncCompletedEventHandler(DownloadFileCompletedArchivos(Filename, runInOpenMSX));
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
            //this.Focus();
            toolStripStatusLabel1.Text = "Descargando " + nombreArchivo + " " + e.ProgressPercentage + "% de ( " + e.BytesReceived + " / " + e.TotalBytesToReceive + ")";
        }
        private readonly static string reservedCharacters = "!*'();:@&=+$,?%#[]";
        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

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
            if (toolStripComboBox3.SelectedItem.ToString() != kakarot.Strings.FiltroRapido) ApplyFilter(toolStripComboBox3.SelectedItem.ToString(), dataGridView1, _dataTableDV1);
        }
        private void fjntr_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClearDirectory(Application.StartupPath + "\\tmp\\");
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
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                //folderDialog.Description = "Seleccione la carpeta donde se encuentra OpenMSX";
                //folderDialog.ShowNewFolderButton = false;

                // Mostrar el cuadro de diálogo
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    //iniciacilzamos config...
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    //config.AppSettings.Settings.Add("NewSetting", "SomeValue");

                    config.AppSettings.Settings["OpenMSXPath"].Value = folderDialog.SelectedPath;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                    MessageBox.Show("La aplicacion se reiniciara", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Restart();
                    Environment.Exit(0);
                }
                else
                {
                    MessageBox.Show("Cancelado por el usuario", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DragOCMfrm ddragocm = new DragOCMfrm();
            ddragocm.ShowDialog();
        }
        private void permitirMultiplesInstanciasToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (permitirMultiplesInstanciasToolStripMenuItem.Checked)
            {
                config.AppSettings.Settings["PermitirVariasInstancias"].Value = "true";
            }
            else
            {
                config.AppSettings.Settings["PermitirVariasInstancias"].Value = "false";
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                string outputPath = Application.StartupPath + "\\tmp\\list.exe";
                string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (string resourceName in resourceNames)
                {
                    if (resourceName.ToLower().Contains("list.exe"))
                    {
                        ExtractEmbeddedResource(resourceName, outputPath);
                    }

                }

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                string file = "";
                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "Archivos basic MSX (*.bas)|*.bas";
                openFileDialog1.FilterIndex = 0;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var path = openFileDialog1.FileName;
                    var solonombre = openFileDialog1.SafeFileName.ToLower();
                    File.Copy(path, Application.StartupPath + "\\tmp\\" + solonombre, true);

                    var prevWorking = Environment.CurrentDirectory;
                    Directory.SetCurrentDirectory(Application.StartupPath + "\\tmp");
                    using (Process process = new Process())
                    {
                        //process.StartInfo.WorkingDirectory = Application.StartupPath + "\\Utils\\";
                        process.StartInfo.FileName = "list.exe";
                        process.StartInfo.Arguments = solonombre;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.CreateNoWindow = true;

                        StringBuilder output = new StringBuilder();
                        StringBuilder error = new StringBuilder();

                        using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                        using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                        {
                            process.OutputDataReceived += (ssender, ee) =>
                            {
                                if (ee.Data == null)
                                {
                                    outputWaitHandle.Set();
                                }
                                else
                                {
                                    output.AppendLine(ee.Data);
                                }
                            };
                            process.ErrorDataReceived += (ssender, ee) =>
                            {
                                if (ee.Data == null)
                                {
                                    errorWaitHandle.Set();
                                }
                                else
                                {
                                    error.AppendLine(ee.Data);
                                }
                            };

                            process.Start();

                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            if (process.WaitForExit(5000) && outputWaitHandle.WaitOne(5000) && errorWaitHandle.WaitOne(5000))
                            {
                                // Process completed. Check process.ExitCode here.
                                File.WriteAllText(path.ToLower().Replace(".bas", ".ASCII"), output.ToString());
                            }
                            else
                            {
                                // Timed out.
                                MessageBox.Show("Ocurrio un error al procesar el archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Environment.CurrentDirectory = prevWorking;
                                return;
                            }
                        }
                    }
                    Environment.CurrentDirectory = prevWorking;
                    //  File.WriteAllText(path.ToLower().Replace(".bas", ".ASCII"), file);
                    DialogResult dialogResult = MessageBox.Show("Archivo generado correctamente.\r\n ¿Quieres abrirlo?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.Yes)
                    {
                        ProcessStartInfo processInfo = new ProcessStartInfo
                        {
                            FileName = path.ToLower().Replace(".bas", ".ASCII"),
                            UseShellExecute = true // Necesario para abrir con la aplicación predeterminada
                        };
                        Process.Start(processInfo);
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            ClearDirectory(Application.StartupPath + "\\tmp\\");
        }
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            listBox1.Items.Clear();
            dictionary.Clear();
            string latabla = "", elrow = "", lacell = "";
            HtmlElementCollection tables = this.webBrowser.Document.GetElementsByTagName("tr");
            foreach (HtmlElement TBL in tables)
            {
                latabla = TBL.InnerText;
                foreach (HtmlElement ROW in TBL.All)
                {
                    elrow = ROW.InnerText;
                    foreach (HtmlElement CELL in ROW.All)
                    {

                        lacell = CELL.InnerText;
                        if (CELL.GetAttribute("href").Trim() != "")
                        {
                            dictionary.Add(latabla, CELL.GetAttribute("href"));
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, string> entry in dictionary)
            {
                listBox1.Items.Add(entry.Key);
            }
            toolStripStatusLabel1.Text = "Total de archivos: " + listBox1.Items.Count.ToString();
            originalItems.Clear();
            foreach (var item in listBox1.Items)
            {
                originalItems.Add(item.ToString());
            }
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, string> entry in dictionary)
            {
                if (entry.Key == listBox1.SelectedItem.ToString())
                {
                    if (listBox1.SelectedItem.ToString().Contains("Directory"))
                    {
                        //Directorio
                        webBrowser.Navigate(entry.Value);
                        break;
                    }
                    else
                    {
                        using (var fbd = new FolderBrowserDialog())
                        {
                            DialogResult result = fbd.ShowDialog();
                            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                            {
                                toolStripStatusLabel1.Text = "Descargando, por favor espere.";
                                string folder = fbd.SelectedPath;
                                string url = entry.Value;
                                string fileName = entry.Key.Substring(0, entry.Key.IndexOf(entry.Key.Substring(entry.Key.Length - 4, 3).Trim()) + 3);
                                string tempfile = Path.GetTempFileName();
                                string DestFolfer = folder + "\\" + fileName.Substring(0, fileName.Length - 4);
                                if (Directory.Exists(DestFolfer))
                                {
                                    DialogResult dialogResult = MessageBox.Show("El directorio de destino ya existe, si pulsa \"SI\" se borrará todo, incluyendo su contenido, ¿esta seguro?", "Advertencia", MessageBoxButtons.YesNo);
                                    if (dialogResult == DialogResult.Yes)
                                    {
                                        //   Directory.Delete(DestFolfer, true);

                                    }
                                    else if (dialogResult == DialogResult.No)
                                    {
                                        MessageBox.Show("Cancelado");
                                        return;

                                    }
                                }
                                TaskDownloadFileArchivos(folder + "\\" + fileName, url, false);

                            }
                        }


                    }
                }

            }
        }
        private void filehunterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PathOpenMSX is not null) OpenMSX.Enabled = true;
            verNovedadesToolStripMenuItem.Checked = false;
            webMSXToolStripMenuItem.Enabled = true;
            toolStripComboBox3.Enabled = true;
            verNovedadesToolStripMenuItem.Enabled = true;
            foreach (ToolStripMenuItem item in reposToolStripMenuItem.DropDownItems) { item.Checked = false; }
            filehunterToolStripMenuItem.Checked = true;
            listBox1.Visible = false;
            listBox2.Visible = false;
            dataGridView1.BringToFront();
            dataGridView1.Visible = true;
            panel1.BringToFront();
            toolStripStatusLabel1.Text = "Total de archivos: " + dataGridView1.RowCount.ToString();

        }
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (PathOpenMSX is not null) OpenMSX.Enabled = false;
            if (listBox1.SelectedIndex == -1) listBox1.SelectedIndex = 0;
            toolStripStatusLabel1.Text = "Total de archivos: " + listBox1.Items.Count.ToString();
            webMSXToolStripMenuItem.Enabled = false;
            toolStripComboBox3.Enabled = false;
            verNovedadesToolStripMenuItem.Enabled = false;
            foreach (ToolStripMenuItem item in reposToolStripMenuItem.DropDownItems) { item.Checked = false; }
            MSXScanstoolStripMenuItem4.Checked = true;
            dataGridView1.Visible = false;
            listBox2.Visible = false;
            listBox1.BringToFront();
            panel1.BringToFront();
            listBox1.Visible = true;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Configurar evento de filtrado en tiempo real
            if (IsControlVisible(dataGridView1))
            {
                ApplyFilter(textBox1.Text, dataGridView1, _dataTableDV1);
                label1.Visible = true;
                checkBox1.Visible = false;
                checkBox2.Visible = false;
                checkBox3.Visible = false;
            }
            if (IsControlVisible(dataGridView2))
            {
                ApplyFilter(textBox1.Text, dataGridView1, _dataTableDV2);
                label1.Visible = false;
                checkBox1.Visible = true;
                checkBox2.Visible = true;
                checkBox3.Visible = true;
            }
            if (IsControlVisible(listBox1))
            {
                ApplyFilter(textBox1.Text, listBox1);
            }
        }
        private void informarDeActualizacionesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (informarDeActualizacionesToolStripMenuItem.Checked)
            {
                config.AppSettings.Settings["InformaDeActualizacionesFileHunter"].Value = "true";
            }
            else
            {
                config.AppSettings.Settings["InformaDeActualizacionesFileHunter"].Value = "false";
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        private void rOMCASToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Archivos ROM (*.rom)|*.rom|Archivos DSK (*.dsk)|*.dsk|Todos los archivos (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string outputPath = Application.StartupPath + "\\tmp\\jakusha.exe";
                    string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                    foreach (string resourceName in resourceNames)
                    {
                        if (resourceName.ToLower().Contains("jakusha.exe"))
                        {
                            ExtractEmbeddedResource(resourceName, outputPath);
                        }

                    }

                    FileInfo fs = new FileInfo(dlg.FileName);
                    long filesize = fs.Length / 1024;
                    if (filesize > 32) { MessageBox.Show("Maximo ROMs de 32Kb.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                    else
                    {
                        File.Copy(dlg.FileName, Application.StartupPath + "\\tmp\\" + dlg.SafeFileName, true);
                        try
                        {
                            ProcessStartInfo psi = new ProcessStartInfo();
                            psi.FileName = Application.StartupPath + "\\tmp\\jakusha.exe";
                            psi.Arguments = "\"" + dlg.SafeFileName + "\" \"" + dlg.SafeFileName.ToLower().Replace(".rom", ".cas") + "\"";
                            psi.UseShellExecute = false;
                            psi.RedirectStandardOutput = true;
                            psi.CreateNoWindow = true;
                            psi.WorkingDirectory = Application.StartupPath + "\\tmp\\";
                            var p = Process.Start(psi);
                            p.WaitForExit();
                            File.Delete(Application.StartupPath + "\\tmp\\" + dlg.SafeFileName);

                            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
                            folderDlg.ShowNewFolderButton = true;

                            // Show the FolderBrowserDialog.  
                            DialogResult result = folderDlg.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                File.Copy(Application.StartupPath + "\\tmp\\" + dlg.SafeFileName.ToLower().Replace(".rom", ".cas"), folderDlg.SelectedPath + "\\" + dlg.SafeFileName.ToLower().Replace(".rom", ".cas"));
                            }
                            File.Delete(Application.StartupPath + "\\tmp\\" + dlg.SafeFileName.ToLower().Replace(".rom", ".cas"));
                            MessageBox.Show("Archivo creado correctamente.\r\n Thanks to スターニャン（元スターマン）@starmanblues", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            File.Delete(dlg.SafeFileName);
                            MessageBox.Show("Error:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("Error lanzando " + dlg.FileName + "\r\n", "Error");
                }
            }
        }
        private void aplicarParcheIPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //sacado de https://sneslab.net/wiki/IPS_patching_code
            try
            {
                string larom = "", rom = "", elips = "", ips = "", elsha = "";
                FileStream romstream, ipsstream;
                var dlg = new OpenFileDialog();
                dlg.Filter = "Archivos ROM (*.rom)|*.rom|Archivos DSK (*.dsk)|*.dsk|Todos los archivos (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    larom = dlg.FileName;
                    rom = Path.GetFileName(larom);
                    elsha = ComputeFileSha1(larom);
                }
                else { return; }
                dlg.Filter = "Archivos ips (*.ips)|*.ips|Todos los archivos (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    elips = dlg.FileName;
                    ips = Path.GetFileName(elips);

                }
                else { return; }
                DialogResult dialogResult = MessageBox.Show($"¿Modificar el archivo {rom}\r\nsha1:{elsha}\r\ncon el parche {ips}?", "Atencion", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    File.Copy(larom, larom.ToLower().Replace(".rom", ".old"));
                    romstream = new FileStream(larom, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    ipsstream = new FileStream(elips, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    int lint = (int)ipsstream.Length;
                    byte[] ipsbyte = new byte[ipsstream.Length];
                    byte[] rombyte = new byte[romstream.Length];
                    IAsyncResult romresult;
                    IAsyncResult ipsresult = ipsstream.BeginRead(ipsbyte, 0, lint, null, null);
                    ipsstream.EndRead(ipsresult);
                    int ipson = 5;
                    int totalrepeats = 0;
                    int offset = 0;
                    bool keepgoing = true;
                    while (keepgoing == true)
                    {
                        offset = ipsbyte[ipson] * 0x10000 + ipsbyte[ipson + 1] * 0x100 + ipsbyte[ipson + 2];
                        ipson++;
                        ipson++;
                        ipson++;
                        if (ipsbyte[ipson] * 256 + ipsbyte[ipson + 1] == 0)
                        {
                            ipson++;
                            ipson++;
                            totalrepeats = ipsbyte[ipson] * 256 + ipsbyte[ipson + 1];
                            ipson++;
                            ipson++;
                            byte[] repeatbyte = new byte[totalrepeats];
                            for (int ontime = 0; ontime < totalrepeats; ontime++)
                                repeatbyte[ontime] = ipsbyte[ipson];
                            romstream.Seek(offset, SeekOrigin.Begin);
                            romresult = romstream.BeginWrite(repeatbyte, 0, totalrepeats, null, null);
                            romstream.EndWrite(romresult);
                            ipson++;
                        }
                        else
                        {
                            totalrepeats = ipsbyte[ipson] * 256 + ipsbyte[ipson + 1];
                            ipson++;
                            ipson++;
                            romstream.Seek(offset, SeekOrigin.Begin);
                            romresult = romstream.BeginWrite(ipsbyte, ipson, totalrepeats, null, null);
                            romstream.EndWrite(romresult);
                            ipson = ipson + totalrepeats;
                        }
                        if (ipsbyte[ipson] == 69 && ipsbyte[ipson + 1] == 79 && ipsbyte[ipson + 2] == 70)
                            keepgoing = false;
                    }
                    romstream.Close();
                    ipsstream.Close();
                    MessageBox.Show("Parche aplicado con exito", "Correcto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (dialogResult == DialogResult.No)
                {
                    // MessageBox.Show("Cancelado por el usuario", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aplicar el parche: \r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static string ComputeFileSha1(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("La ruta del archivo no puede estar vacía.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("El archivo especificado no existe.", filePath);
            }

            using (var sha1 = System.Security.Cryptography.SHA1.Create())
            using (var fileStream = File.OpenRead(filePath))
            {
                var hashBytes = sha1.ComputeHash(fileStream);
                return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToUpperInvariant();
            }
        }
        private void verSHA1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Archivos ROM (*.rom)|*.rom|Archivos DSK (*.dsk)|*.dsk|Todos los archivos (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // romname is the original ROM, patchname is the patch to apply
                MessageBox.Show(ComputeFileSha1(dlg.FileName), "SHA1");
            }
        }
        private void Sender_DataSent(object sender, int e)
        {
            bytesSent += e - 2;
            //var pos = Console.CursorLeft;
            toolStripStatusLabel1.Text = acumulador + $"{((decimal)bytesSent / fileLength) * 100:0.0}%";
            //Console.CursorLeft = pos;
        }
        private void Sender_HeaderSent(object sender, (long, string) e)
        {
            fileLength = e.Item1;
            filenameSent = e.Item2;
            acumulador = $"Enviando archivo --> {filenameSent}, tamaño {(decimal)fileLength / 1024:0}K... ";
        }
        private void enviaArchivoSeleccionadoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void enviaArchivoLocalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Si algun dia tengo manera de que me funcione lo añado, mientras...
            //no tengo manera humana de que me funcione con
            //niguno de lo tres usb ttl chinescos que tengo tirados por casa
            var dlg = new OpenFileDialog();
            Sender sendero;
            SerialPort serialPort = new SerialPort();
            dlg.Filter = "Archivos ROM (*.rom)|*.rom|Archivos DSK (*.dsk)|*.dsk|Todos los archivos (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    serialPort.PortName = toolStripComboBox2.SelectedItem.ToString();
                    if (serialPort.IsOpen) { serialPort.Close(); }

                    //sacado de https://github.com/Konamiman/JoySerTrans
                    acumulador = "";
                    sendero = new Sender("COM6", 19200);
                    sendero.HeaderSent += Sender_HeaderSent;
                    sendero.DataSent += Sender_DataSent;
                    sendero.Send(dlg.FileName, "1.ROM");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrio un error al enviar el archivo:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    serialPort.Dispose();
                }
            }
        }
        private void dragDropdskExploresrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Abriendo .DSK, por favor, espere...";
            var dlg = new OpenFileDialog();
            dlg.Filter = "Archivos DSK (*.dsk)|*.dsk|Todos los archivos (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FrmMiniExplorer frm = new FrmMiniExplorer(dlg.FileName);
                frm.Show();
            }
            else
            {
                toolStripStatusLabel1.Text = "Cancelado por el usuario...";
            }
        }
        private void IdiomaSpatoolStripMenuItem_Click(object sender, EventArgs e)
        {
            IdiomaEngtoolStripMenuItem.Checked = false;
            IdiomaSpatoolStripMenuItem.Checked = true;
            ApplyLanguage("es");
            UpdateTexts();
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["Idioma"].Value = "es";
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        private void IdiomaEngtoolStripMenuItem5_Click(object sender, EventArgs e)
        {
            IdiomaSpatoolStripMenuItem.Checked = false;
            IdiomaEngtoolStripMenuItem.Checked = true;
            ApplyLanguage("en");
            UpdateTexts();
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["Idioma"].Value = "en";
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        private bool IsUrl(string text)
        {
            string urlPattern = @"^(https?:\/\/[^\s]+)$";
            return Regex.IsMatch(text, urlPattern, RegexOptions.IgnoreCase);
        }
        private void HandleUrl(string url)
        {

            var urls = File.ReadAllLines(Application.StartupPath + "\\favoritos.fav");
            if (Array.Exists(urls, u => u.Equals(url, StringComparison.OrdinalIgnoreCase)))
            {
                this.TopMost = true;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
                var result = MessageBox.Show(
                    $"La URL ya existe:\n\n{url}\n\n¿Desea eliminarla?",
                    "URL Duplicada",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    File.WriteAllLines(Application.StartupPath + "\\favoritos.fav", Array.FindAll(urls, u => !u.Equals(url, StringComparison.OrdinalIgnoreCase)));
                    MessageBox.Show("URL eliminada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clipboard.Clear();
                    RemoveSubMenuItemToolStrip(Favoritos, url);
                }
            }
            else
            {
                this.TopMost = true;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
                var result = MessageBox.Show(
                   $"Detectada un URL en el portapapeles:\n\n{url}\n\n¿Añadirla a favoritos?",
                   "Atencion",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question
               );

                if (result == DialogResult.Yes)
                {
                    File.AppendAllText(Application.StartupPath + "\\favoritos.fav", url + Environment.NewLine);
                    MessageBox.Show("URL añadida a favoritos.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clipboard.Clear();
                    AddSubMenuItemToolStrip(Favoritos, url, false, clickedItem =>
                    {
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }, url);

                }

            }
            this.TopMost = false;
        }
        private void InitializeClipboardMonitor()
        {
            clipboardMonitorTimer = new System.Windows.Forms.Timer
            {
                Interval = 500 // Verificar cada 500 ms
            };

            clipboardMonitorTimer.Tick += ClipboardMonitorTimer_Tick;
            clipboardMonitorTimer.Start();
        }
        private void ClipboardMonitorTimer_Tick(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string currentText = Clipboard.GetText();

                if (currentText != lastClipboardText && IsUrl(currentText))
                {
                    lastClipboardText = currentText;
                    HandleUrl(currentText);
                }
            }
        }
        private void GhLevel1Async(string repo)
        {
            string contenido = "";
            //Dalekamistoso/MFC-PUBLIC
            //https://api.github.com/repos/markheath/azure-deploy-manage-containers/contents
            //https://api.github.com/repos/Dalekamistoso/MFC-PUBLIC/contents
            System.Threading.Tasks.Task.Run(async () =>
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                var contentsUrl = repo;// $"https://api.github.com/repos/{repo}/contents/";
                var contentsJson = await httpClient.GetStringAsync(contentsUrl);
                var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);
                foreach (var file in contents)
                {
                    var fileType = (string)file["type"];
                    if (fileType == "dir")
                    {
                        var directoryContentsUrl = (string)file["url"];
                        // use this URL to list the contents of the folder
                        contenido += ($"DIR:{directoryContentsUrl}\r\n");
                    }
                    else if (fileType == "file")
                    {
                        var downloadUrl = (string)file["download_url"];
                        // use this URL to download the contents of the file
                        contenido += ($"{downloadUrl}\r\n");
                    }
                }

            }).ContinueWith(t => acabado(contenido));
        }
        private void GhLevel2Async(string repo)
        {
            string contenido = "";
            //https://api.github.com/repos/markheath/azure-deploy-manage-containers/contents
            //https://api.github.com/repos/Dalekamistoso/MFC-PUBLIC/contents
            System.Threading.Tasks.Task.Run(async () =>
            {
                Random rnd = new Random();
                int uno = rnd.Next(100, 714);  // creates a number between 1 and 12
                int dos = rnd.Next(100, 414);   // creates a number between 1 and 6
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(uno.ToString(), dos.ToString()));
                //var contentsUrl = $"https://api.github.com/repos/{repo}/contents/";
                var contentsUrl = $"{repo}";
                var contentsJson = await httpClient.GetStringAsync(contentsUrl);
                var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);
                foreach (var file in contents)
                {
                    var fileType = (string)file["type"];
                    if (fileType == "dir")
                    {
                        var directoryContentsUrl = (string)file["url"];
                        // use this URL to list the contents of the folder
                        contenido += ($"DIR:{directoryContentsUrl}\r\n");
                    }
                    else if (fileType == "file")
                    {
                        var downloadUrl = (string)file["download_url"];
                        // use this URL to download the contents of the file
                        contenido += ($"{downloadUrl}\r\n");
                    }
                }

            }).ContinueWith(t => acabado(contenido));

        }
        private void acabado(string resultado)
        {
            this.Invoke((System.Windows.Forms.MethodInvoker)(() => listBox2.Items.Add("\\.")));
            this.Invoke((System.Windows.Forms.MethodInvoker)(() => listBox2.Items.Add("\\..")));
            string[] lines = resultado.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Trim() != "") this.Invoke((System.Windows.Forms.MethodInvoker)(() => listBox2.Items.Add(line)));
            }

        }
        private void Preparalistbox2(string urlrepo)
        {
            repoenuso = urlrepo;
            listBox2.Items.Clear();
            ConfigureListBoxAppearance(listBox2);
            if (PathOpenMSX is not null) OpenMSX.Enabled = false;
            if (listBox2.SelectedIndex == -1) listBox1.SelectedIndex = 0;
            toolStripStatusLabel1.Text = "Total de archivos: " + listBox2.Items.Count.ToString();
            webMSXToolStripMenuItem.Enabled = false;
            toolStripComboBox3.Enabled = false;
            verNovedadesToolStripMenuItem.Enabled = false;
            MSXScanstoolStripMenuItem4.Checked = false;
            dataGridView1.Visible = false;
            listBox1.Visible = false;
            listBox2.BringToFront();
            panel1.BringToFront();
            listBox2.Visible = true;
            GhLevel1Async(urlrepo);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                if (listBox2.SelectedItem.ToString() != ("\\.") && listBox2.SelectedItem.ToString() != ("\\.."))
                {
                    string decodedUrl = Uri.UnescapeDataString(listBox2.SelectedItem.ToString().Replace("DIR:", "").Replace("?ref=main", ""));
                    if (!InvokeRequired)
                    {
                        toolStripStatusLabel1.Text = "GitHub " + decodedUrl.Substring(decodedUrl.LastIndexOf("/") + 1, (decodedUrl.Length - 1) - decodedUrl.LastIndexOf("/"));
                    }
                    else
                    {
                        this.Invoke((System.Windows.Forms.MethodInvoker)(() => toolStripStatusLabel1.Text = "GitHub " + decodedUrl));
                    }
                }
            }
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                if (listBox2.SelectedItem.ToString() == ("\\."))
                {
                    memoriarutas = "";
                    nivelAnterior = "";
                    nivelAnteriorPre = "";
                    nivelActual = "";
                    listBox2.Items.Clear();
                    GhLevel1Async(repoenuso);
                    return;
                }
                if (listBox2.SelectedItem.ToString() == ("\\.."))
                {
                    //Subimos un nivel
                    int i = -1;
                    string[] Coincidencias = memoriarutas.Split(new string[] { "|_|" }, StringSplitOptions.None);
                    foreach (string line in Coincidencias)
                    {
                        i++;
                    }
                    if (memoriarutas != "" && i > 1)
                    {
                        nivelActual = memoriarutas.Substring(memoriarutas.LastIndexOf("|_|"), memoriarutas.Length - memoriarutas.LastIndexOf("|_|"));
                        nivelAnteriorPre = memoriarutas.Replace(nivelActual, "");
                        nivelAnterior = nivelAnteriorPre.Substring(nivelAnteriorPre.LastIndexOf("|_|"), nivelAnteriorPre.Length - nivelAnteriorPre.LastIndexOf("|_|"));
                        memoriarutas = memoriarutas.Replace(nivelActual, "");
                        listBox2.Items.Clear();
                        GhLevel2Async(nivelAnterior.Replace("DIR:", "").Replace("|_|", ""));
                    }
                    else
                    {
                        memoriarutas = "";
                        nivelAnterior = "";
                        nivelAnteriorPre = "";
                        nivelActual = "";
                        listBox2.Items.Clear();
                        GhLevel1Async(repoenuso);
                    }
                    return;
                }

                else
                {
                    if (listBox2.SelectedItem.ToString().StartsWith("DIR:"))
                    {
                        GhLevel2Async(listBox2.SelectedItem.ToString().Replace("DIR:", ""));
                        memoriarutas += "|_|" + listBox2.SelectedItem.ToString();
                        listBox2.Items.Clear();
                    }
                    else
                    {
                        using (var fbd = new FolderBrowserDialog())
                        {
                            DialogResult result = fbd.ShowDialog();
                            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                            {
                                toolStripStatusLabel1.Text = "-Descargando, por favor espere-";
                                string folder = fbd.SelectedPath;
                                // string NoeEncUrl = UrlEncode(listBox1.SelectedItem.ToString().Substring(2, listBox1.SelectedItem.ToString().Length - 2)).Replace("\\", "/").Replace(" ", "%20");
                                string url = listBox2.SelectedItem.ToString();
                                string decodedUrl = Uri.UnescapeDataString(listBox2.SelectedItem.ToString().Replace("DIR:", "").Replace("?ref=main", ""));
                                string fileName = decodedUrl.Substring(decodedUrl.LastIndexOf("/") + 1, (decodedUrl.Length - 1) - decodedUrl.LastIndexOf("/"));
                                if (url.EndsWith("yaml"))
                                {
                                    url = yamlToURLDown(url).Trim();
                                    fileName = url.Substring(url.LastIndexOf("/") + 1, (url.Length - 1) - url.LastIndexOf("/"));
                                }
                                string tempfile = Path.GetTempFileName();
                                string strPathnoFilename = Path.GetFileNameWithoutExtension(fileName);
                                string DestFolfer = folder + "\\" + strPathnoFilename + "\\";
                                if (Directory.Exists(DestFolfer))
                                {
                                    DialogResult dialogResult = MessageBox.Show("El directorio de destino ya existe, si pulsa \"SI\" se borrará todo, incluyendo su contenido, ¿esta seguro?", "Advertencia", MessageBoxButtons.YesNo);
                                    if (dialogResult == DialogResult.Yes)
                                    {
                                        Directory.Delete(DestFolfer, true);

                                    }
                                    else if (dialogResult == DialogResult.No)
                                    {
                                        MessageBox.Show("Cancelado");
                                        return;

                                    }
                                }
                                TaskDownloadFileArchivos(fbd.SelectedPath+"\\"+fileName,url, false);
                            }
                        }


                    }
                }
            }
        }
        private string yamlToURLDown(string urlyaml)
        {
            string strContent = "";
            var webRequest = WebRequest.Create(urlyaml);
            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                strContent = reader.ReadToEnd();
            }

            // yml = "---\r\nname: '1937'\r\nversion: '1.0'\r\nrelease: 1\r\nsummary: '1937 (MSXdev''22)'\r\nauthor: 'joesg'\r\npackage_author: 'Carles Amigó (fr3nd)'\r\nlicense: 'Freeware'\r\ncategory: 'Games'\r\nsystem: 'MSX'\r\nrequirements:\r\n  - 'ROM'\r\nurl: 'https://www.msxdev.org/2022/10/04/msxdev22-30-1937/'\r\ndescription: |\r\n  # 1937\r\n\r\n  The game is about a kid that tries to escape from the horror.\r\ninstalldir: '\\1937'\r\nfiles:\r\n  - 1937.zip: 'https://www.msxdev.org/wp-content/uploads/2022/10/MSXdev22_1937_v1.0.zip'\r\nbuild: |\r\n  mkdir -p package/\r\n  unzip 1937.zip\r\n  mv \"MSXdev22_1937_v1.0_eng.rom\" package/1937.ROM\r\n  cat > package/1937.BAT << EOF\r\n  REM Look for SROM.COM in default dir\r\n  IFF EXIST %_DISK%:\\SOFAROM\\SROM.COM\r\n    %_DISK%:\\SOFAROM\\SROM.COM 1937.ROM\r\n  ELSE\r\n    REM Try to run it from path\r\n    IFF EXIST SROM.COM\r\n      SROM.COM 1937.ROM\r\n    ELSE\r\n      echo \"SROM.COM not found. Install it with: 'HUB install SOFAROM'\"\r\n      exit 1\r\n    ENDIFF\r\n  ENDIFF\r\n  EOF\r\n  unix2dos package/1937.BAT\r\nchangelog: |\r\n  - 1.0-1 2022-10-11\r\n    - First release";
            string[] lines = strContent.Split(new string[] { "files:" }, StringSplitOptions.None);
            int a = 0;
            foreach (string value in lines)
            {
                a++;
                if (a == 2)
                {
                    int b = 0;
                    //MessageBox.Show(value.Substring(0, value.IndexOf("build:")));
                    string casi = value.Substring(0, value.IndexOf("build:"));
                    string[] array = casi.Split(new string[] { "'" }, StringSplitOptions.None);
                    foreach (string value2 in array)
                    {
                        b++;
                        if (b == 2) return value2;
                    }
                }
            }
            return "Not found";
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

