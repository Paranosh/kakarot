using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kakarot
{
    public partial class DragOCMfrm : Form
    {
        public DragOCMfrm()
        {
            InitializeComponent();
        }

        private void button1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string extension = "";
                Compartida.TodosLosArchivos = "";
                Compartida.NombreArchivo = "";
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var result = from name in files orderby name select name;
                string archivotodo = files[0].ToString();
                string directorio = archivotodo.Substring(0, archivotodo.LastIndexOf("\\") + 1);
                string archivo = archivotodo.Substring((archivotodo.LastIndexOf("\\") + 1), (archivotodo.Length - 1) - archivotodo.LastIndexOf("\\")).Replace(".dsk", "");
                Compartida.NombreArchivo = archivo;
                foreach (var value in result)
                {
                    if (value != "") Compartida.TodosLosArchivos += value + "\r\n";
                }
                Form nombarch = new frmNombreArchivo();
                nombarch.ShowDialog();
                if (Compartida.createrom) { extension = "rom"; }
                if (!Compartida.createrom) { extension = "dsk"; }
                if (Compartida.NombreArchivo == "")
                {
                    //Cancelled, no name specified
                    MessageBox.Show("No se creará ningún archivo", "Cancelado");
                }
                else
                {
                    if (!File.Exists(directorio + Compartida.NombreArchivo + "." + extension))
                    {

                        using (var outputStream = File.Create(directorio + Compartida.NombreArchivo + "." + extension))
                        {
                            int a = 0;
                            foreach (var value in result)
                            {
                                a++;
                                using (var inputStream = File.OpenRead(value))
                                {
                                    if (a == 1)
                                    {
                                        if (Compartida.createrom)
                                        {
                                            using (var inputrom = File.OpenRead(Directory.GetCurrentDirectory() + "\\dsk2rom.rom"))
                                            {
                                                inputrom.CopyTo(outputStream);
                                            }
                                            inputStream.CopyTo(outputStream);
                                        }
                                        else { inputStream.CopyTo(outputStream); }
                                    }
                                    else
                                    {
                                        inputStream.CopyTo(outputStream);
                                    }
                                }
                                if (checkBox1.Checked) File.Delete(value);
                            }
                            var userResult = AutoClosingMessageBox.Show("Fichero creado correctamente", "OK", 1000, MessageBoxButtons.OK);
                            if (userResult == System.Windows.Forms.DialogResult.OK)
                            {
                                // do something
                                LanzaDescargado(directorio + Compartida.NombreArchivo + "." + extension);
                            }
                        }
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Ya existe un archivo con el mismo nombre, ¿estas seguro?", "¿Sobreescribir?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            File.Delete(directorio + Compartida.NombreArchivo + "." + extension);
                            using (var outputStream = File.Create(directorio + Compartida.NombreArchivo + "." + extension))
                            {
                                int a = 0;
                                foreach (var value in result)
                                {
                                    a++;
                                    using (var inputStream = File.OpenRead(value))
                                    {
                                        if (a == 1)
                                        {
                                            if (Compartida.createrom)
                                            {
                                                using (var inputrom = File.OpenRead(Directory.GetCurrentDirectory() + "\\dsk2rom.rom"))
                                                {
                                                    inputrom.CopyTo(outputStream);
                                                }
                                                inputStream.CopyTo(outputStream);
                                            }
                                            else { inputStream.CopyTo(outputStream); }
                                        }
                                        else
                                        {
                                            inputStream.CopyTo(outputStream);
                                        }
                                    }
                                    if (checkBox1.Checked) File.Delete(value);
                                }
                            }
                            var userResult = AutoClosingMessageBox.Show("Fichero creado correctamente", "OK", 1000, MessageBoxButtons.OK);
                            if (userResult == System.Windows.Forms.DialogResult.OK)
                            {
                                // do something
                                LanzaDescargado(directorio + Compartida.NombreArchivo + "." + extension);
                            }
                           
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            //do something else
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogResult dialogResult = MessageBox.Show("Uppps...algo fue mal, te muestro el error?", "Error", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    // do something
                    MessageBox.Show(ex.ToString(), "Error");
                }
                else { }
            }
        }

        private void button1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                bool sondsk = false;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var result = from name in files orderby name select name;
                string archivotodo = files[0].ToString();
                string directorio = archivotodo.Substring(0, archivotodo.LastIndexOf("\\") + 1);
                string archivo = archivotodo.Substring((archivotodo.LastIndexOf("\\") + 1), (archivotodo.Length - 1) - archivotodo.LastIndexOf("\\")).Replace(".dsk", "");
                foreach (var value in result)
                {
                    if (value.ToLower().EndsWith(".dsk")) { sondsk = true; } else { sondsk = false; break; }

                }
                
                if (sondsk) e.Effect = DragDropEffects.Link;
                else
                    e.Effect = DragDropEffects.None;
            }
            else
            { e.Effect = DragDropEffects.None; }
        }

        private void DragOCMfrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Compartida.concatenarformularioabierto = false;
        }
        private void LanzaDescargado(string rutaconnombre)
        {
            if (Compartida.AbrirOpenMsx)
            {
                try
                {
                    string machine = Compartida.MaquinaOpenMsx;
                    if (rutaconnombre.ToLower().EndsWith(".rom"))
                    {

                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = Application.StartupPath + "\\openmsx\\openmsx.exe";
                        psi.Arguments = "-machine " + machine + " -cart \"" + rutaconnombre + "\"";
                        psi.CreateNoWindow = true;
                        psi.WorkingDirectory = Application.StartupPath + "\\openmsx\\";
                        var p = Process.Start(psi);
                    }
                    else if (rutaconnombre.ToLower().EndsWith(".dsk"))
                    {

                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = Application.StartupPath + "\\openmsx\\openmsx.exe";
                        psi.Arguments = "-machine " + machine + " \"" + rutaconnombre + "\"";
                        psi.CreateNoWindow = true;
                        psi.WorkingDirectory = Application.StartupPath + "\\openmsx\\";
                        var p = Process.Start(psi);
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(@rutaconnombre);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        private void DragOCMfrm_Load(object sender, EventArgs e)
        {

        }
    }
}
