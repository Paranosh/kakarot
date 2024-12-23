using System.ComponentModel;
using System.Text;

namespace kakarot
{
    public partial class FrmMiniExplorer : Form
    {
        private VFATOperation fop;
        string Archivo;
        bool cerrarAlAbrir=false;
        public FrmMiniExplorer(string archivo)
        {
            InitializeComponent();
            Archivo= archivo.ToLower();
        }

        private void FrmMiniExplorer_Load(object sender, EventArgs e)
        {
            ConfigureListViewAppearance(lvwVDFile);
            AdjustColumnWidths(lvwVDFile);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("Shift-JIS");
            this.lvwVDFile.FullRowSelect = true;
            this.lvwVDFile.View = System.Windows.Forms.View.Details;
            ColumnHeader columnHeader1 = new ColumnHeader();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();
            ColumnHeader columnHeader4 = new ColumnHeader();
            columnHeader1.Text = "NOMBRE";
            columnHeader1.Width = 200;
            columnHeader1.TextAlign = HorizontalAlignment.Left;
            columnHeader2.Text = "MODIFICADO";
            columnHeader2.Width = 200;
            columnHeader2.TextAlign = HorizontalAlignment.Left;
            columnHeader3.Text = "TIPO";
            columnHeader3.Width = 60;
            columnHeader3.TextAlign = HorizontalAlignment.Left;
            columnHeader4.Text = "TAMAÑO";
            columnHeader4.TextAlign = HorizontalAlignment.Right;
            columnHeader4.Width = 50;
            this.lvwVDFile.Columns.AddRange(new ColumnHeader[4]
            {
        columnHeader1,
        columnHeader2,
        columnHeader3,
        columnHeader4
            });
            //this.lvwVDFile.Font = new Font("MS UI Gothic", 9f);
            
            try
            {
                this.fop = (VFATOperation)new DSKFATOperation(Archivo);
                this.VDFileListShow();
            }
            catch (Exception ex) 
            {
                MessageBox.Show("No fue posible abrir el archivo: "+ex.Message,"ERROR", MessageBoxButtons.OK,MessageBoxIcon.Error);
                bgEsperarError.RunWorkerAsync();
            }
           
        }
        private void ConfigureListViewAppearance(ListView listView)
        {
            // Configuración general
            listView.View = View.Details; // Vista en modo detalles
            listView.FullRowSelect = true; // Selección completa de fila
            listView.GridLines = true; // Mostrar líneas de rejilla
            listView.BackColor = Color.DarkGray; // Fondo del ListView
            listView.ForeColor = Color.Black;
            listView.Font = new Font("Arial", 10F, FontStyle.Regular);

            // Habilitar dibujo personalizado
            listView.OwnerDraw = true;

            // Evento DrawColumnHeader para personalizar el encabezado
            listView.DrawColumnHeader += (sender, e) =>
            {
                e.Graphics.FillRectangle(Brushes.Peru, e.Bounds); // Fondo del encabezado
                e.Graphics.DrawString(e.Header.Text, e.Font, Brushes.White, e.Bounds, StringFormat.GenericDefault);
                e.DrawDefault = false;
            };

            // Evento DrawItem para personalizar las filas
            listView.DrawItem += (sender, e) =>
            {
                // No hacemos nada aquí porque se maneja en DrawSubItem
                e.DrawDefault = true;
            };

            // Evento DrawSubItem para personalizar el contenido
            listView.DrawSubItem += (sender, e) =>
            {
                bool isSelected = (e.ItemState & ListViewItemStates.Selected) == ListViewItemStates.Selected;

                // Colores de fondo y texto
                Color backgroundColor = isSelected ? Color.Peru : (e.ItemIndex % 2 == 0 ? Color.Wheat : Color.Moccasin);
                Color textColor = isSelected ? Color.White : Color.Black;

                // Dibujar fondo de la celda
                e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);

                // Dibujar texto del subelemento
                e.Graphics.DrawString(
                    e.SubItem.Text,
                    e.Item.Font,
                    new SolidBrush(textColor),
                    e.Bounds.Location
                );

                // Dibujar líneas horizontales (opcional)
                using (Pen linePen = new Pen(Color.Black, 1))
                {
                    e.Graphics.DrawLine(linePen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                }

                e.DrawFocusRectangle(e.Bounds);
            };
        }

        // Ajustar el ancho de las columnas al contenido
        private void AdjustColumnWidths(ListView listView)
        {
            using (Graphics g = listView.CreateGraphics())
            {
                for (int i = 0; i < listView.Columns.Count; i++)
                {
                    int maxWidth = 0;

                    // Medir el ancho del encabezado
                    string headerText = listView.Columns[i].Text;
                    maxWidth = (int)g.MeasureString(headerText, listView.Font).Width;

                    // Medir el ancho del contenido en cada fila
                    foreach (ListViewItem item in listView.Items)
                    {
                        string cellText = item.SubItems[i].Text;
                        int cellWidth = (int)g.MeasureString(cellText, listView.Font).Width;
                        maxWidth = Math.Max(maxWidth, cellWidth);
                    }

                    // Establecer el ancho de la columna
                    listView.Columns[i].Width = maxWidth + 20; // Añadimos un margen para mayor claridad
                }
            }
        }
        private void AdjustFormWidthToColumns(Form form, ListView listView, int columnCount)
        {
            int totalWidth = 0;

            // Sumar el ancho de las columnas visibles (hasta columnCount)
            for (int i = 0; i < columnCount && i < listView.Columns.Count; i++)
            {
                totalWidth += listView.Columns[i].Width;
            }

            // Añadir el margen del borde del formulario
            int formBorderWidth = form.Width - form.ClientSize.Width;
            int formBorderHeight = form.Height - form.ClientSize.Height;

            // Ajustar el ancho del formulario según el total calculado
            form.Width = totalWidth + formBorderWidth+20;

            // (Opcional) Ajustar también la altura si es necesario
            form.Height = listView.Height + formBorderHeight;
        }

        private string[] GetListFileNames(System.Windows.Forms.ListView list, ref bool Error)
        {
            string[] array = new string[1];
            string tempPath = Path.GetTempPath();
            int index1 = 0;
            Error = false;
            foreach (ListViewItem selectedItem in list.SelectedItems)
            {
                int index2 = int.Parse(selectedItem.SubItems[4].Text);
                if (this.fop.ReadFileFromVDisk(this.fop.CurDir[index2], tempPath, FileMode.Create))
                {
                    Array.Resize<string>(ref array, index1 + 1);
                    array[index1] = Path.Combine(tempPath, this.fop.CurDir[index2].Name);
                    ++index1;
                }
                else
                    Error = true;
            }
            return array;
        }
        private void VDFileListShow()
        {
            string[] items = new string[5] { "", "", "", "", "" };
            this.lvwVDFile.Items.Clear();
            lvwVDFile.Items.Add(".");
            lvwVDFile.Items.Add("..");
            for (int index = 0; index < this.fop.CurDir.Length; ++index)
            {
                if (this.fop.CurDir[index].UsingFlag && ((int)this.fop.CurDir[index].Attribute & 8) == 0)
                {
                    int imageIndex;
                    if (((int)this.fop.CurDir[index].Attribute & 16) != 0)
                    {
                        items[0] = this.fop.CurDir[index].Name + "<DIR>";
                        items[1] = this.fop.CurDir[index].WriteDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                        items[2] = "Folder";
                        items[3] = "";
                        items[4] = index.ToString();
                        imageIndex = 1;
                    }
                    else
                    {
                        items[0] = this.fop.CurDir[index].Name;
                        items[1] = this.fop.CurDir[index].WriteDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                        items[2] = !items[0].Contains(".") ? "File" : Path.GetExtension(items[0]).Substring(1) + " File";
                        items[3] = this.fop.CurDir[index].Size.ToString();
                        items[4] = index.ToString();
                        imageIndex = 3;
                    }
                    if (((int)this.fop.CurDir[index].Attribute & 16) == 0 || !this.fop.CurDir[index].Name.StartsWith("."))
                        this.lvwVDFile.Items.Add(new ListViewItem(items, imageIndex));
                }
            }
            lvwVDFile.Columns[0].Width = -1;
            lvwVDFile.Columns[1].Width = -1;
            lvwVDFile.Columns[2].Width = -1;
            lvwVDFile.Columns[3].Width = 70;
            this.toolStripStatusLabel1.Text = "Espacio en Disco: " + this.fop.DiskSpace.ToString("#,0") + " Byte, Libres:" + this.fop.DiskFreeSpace.ToString("#,0") + " Byte";
            AdjustFormWidthToColumns(this, lvwVDFile, 4);
        }

        private void lvwVDFile_DoubleClick(object sender, EventArgs e)
        {
            if (this.lvwVDFile.SelectedItems.Count != 1)
                return;


            if (this.lvwVDFile.SelectedItems[0].Text == ".")
            {
                this.fop.GetSelectedDirectory(0U);
                this.VDFileListShow();
                return;
            }
            if (this.lvwVDFile.SelectedItems[0].Text == "..")
            {

                this.fop.GetSelectedDirectory(0U);
                this.VDFileListShow();
                return;
            }
            int index = int.Parse(this.lvwVDFile.SelectedItems[0].SubItems[4].Text);
            if (((int)this.fop.CurDir[index].Attribute & 16) == 0)
            {
                //Aqui podriamos meter el rename al hacer doble click
               // var a = 0;
                //(int)this.fop.Rename.CurDir[index]
               // this.fop.Rename("pepe.rom", (uint)index);
               // this.fop.WriteFileToVDisk("pepe.rom");
                return;
            }
            this.fop.GetSelectedDirectory(this.fop.CurDir[index].FirstClusterNo);
            this.VDFileListShow();
        }

        private void lvwVDFile_DragDrop(object sender, DragEventArgs e)
        {
            foreach (string str in (string[])e.Data.GetData(DataFormats.FileDrop, false))
            {
                    if (File.Exists(str))
                    {
                        if ((long)this.fop.DiskFreeSpace < WinFileSystemUtils.GetFileSize(str))
                        {
                            int num = (int)MessageBox.Show("Sin espacio en disco, borre algun archivo y pruebe de nuevo", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            break;
                        }
                        if (!this.fop.FileExists(str))
                            this.fop.WriteFileToVDisk(str);
                        else if (MessageBox.Show("El archivo ya existe, ¿sobrescribir?", "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            this.fop.WriteFileToVDisk(str);
                    }
                    else if (Directory.Exists(str))
                    {
                        if (this.fop.DiskFormatType == VFATOperation.FormatType.MSX_DOS1)
                        {
                            int num = (int)MessageBox.Show("El formato del disco es MSX1, El directorio no puede ser copiado", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        }
                        if ((long)this.fop.DiskFreeSpace < WinFileSystemUtils.GetFolderSize(str))
                        {
                            int num = (int)MessageBox.Show("Sin espacio en disco, borre algun archivo y pruebe de nuevo", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            break;
                        }
                        this.fop.WriteFileToVDisk(str);
                    }
            }
            this.VDFileListShow();
        }

        private void lvwVDFile_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(ListViewItem)))
                return;
            this.lvwVDFile.PointToClient(new Point(e.X, e.Y));
            if ((e.KeyState & 4) > 0)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                if (e.KeyState != 1 && (e.KeyState & 8) <= 0)
                    return;
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void lvwVDFile_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (lvwVDFile.Items[0].Text==".")lvwVDFile.Items[0].Selected = false;
            if (lvwVDFile.Items[1].Text == "..") lvwVDFile.Items[1].Selected = false;
            Path.GetTempPath();
            bool Error = false;
            string[] listFileNames = this.GetListFileNames((System.Windows.Forms.ListView)sender, ref Error);
            if (((IEnumerable<string>)listFileNames).Count<string>() > 0)
            {
                int num = (int)this.lvwVDFile.DoDragDrop((object)new DataObject(DataFormats.FileDrop, (object)listFileNames), DragDropEffects.Copy);
            }
            this.DeleteTemporaryFile(listFileNames);
            if (!Error)
                return;
            int num1 = (int)MessageBox.Show("Error al extraer algun archivo/directorio", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);


        }
        private void DeleteTemporaryFile(string[] PathFiles)
        {
            try
            {
                foreach (string pathFile in PathFiles)
                {
                    if (File.GetAttributes(pathFile).HasFlag((Enum)FileAttributes.Directory))
                        Directory.Delete(pathFile, true);
                    else
                        File.Delete(pathFile);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void lvwVDFile_DragEnter(object sender, DragEventArgs e)
        {
            if (this.fop != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private void lvwVDFile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Delete)
                return;
            this.DeleteVDFolderFiles();
        }
        private void DeleteVDFolderFiles()
        {
            if (this.lvwVDFile.SelectedItems.Count <= 0 || MessageBox.Show("¿Estas seguro?", "CONFIRMAR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            bool flag = true;
            foreach (ListViewItem selectedItem in this.lvwVDFile.SelectedItems)
            {
                if (!this.fop.DeleteVDiskDirFile(this.fop.CurDir[int.Parse(selectedItem.SubItems[4].Text)]))
                    flag = false;
            }
            if (!flag)
            {
                int num = (int)MessageBox.Show("Error al Borrar el archivo/directorio", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            this.VDFileListShow();
        }

        private void FrmMiniExplorer_FormClosed(object sender, FormClosedEventArgs e)
        {
          if (fop !=null) this.fop.Dispose();
        }

        private void bgEsperarError_DoWork(object sender, DoWorkEventArgs e)
        {
            Task.Run(() => System.Threading.Thread.Sleep(10)).Wait();
        }

        private void bgEsperarError_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void bgEsperarError_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }
    }
}
