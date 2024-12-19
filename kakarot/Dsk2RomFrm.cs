using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kakarot
{
    public partial class Dsk2RomFrm : Form
    {
        public Dsk2RomFrm()
        {
            InitializeComponent();
        }
        string archivo = "";
        string archivosinextension = "";
        string argumentos = "";
        string argumentoCompresion = "";
        private void Dsk2RomFrm_Load(object sender, EventArgs e)
        {
            //Seleccionaremos el archivo a crear
            DialogResult result = openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "dsk files (*.dsk)|*.dsk|Todos los archivos (*.*)|*.*";
            if (result == DialogResult.OK) // Test result.
            {

                archivosinextension = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                archivo = openFileDialog1.FileName;
                groupBox1.Text = openFileDialog1.SafeFileName;
                // File.Move(archivo, Application.StartupPath+"\\"+ archivo.Replace(" ", "_"));
            }
            else
            {
                this.Close();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //siempre tiene que ser el ultimo
            if (checkBox1.Checked) { argumentoCompresion = "c 2"; } else { argumentoCompresion = ""; }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) { argumentos = argumentos + "a"; } else { argumentos = argumentos.Replace("a", ""); }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) { argumentos = argumentos + "5"; } else { argumentos = argumentos.Replace("5", ""); }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked) { argumentos = argumentos + "6"; } else { argumentos = argumentos.Replace("6", ""); }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked) { argumentos = argumentos + "p"; } else { argumentos = argumentos.Replace("p", ""); }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked) { argumentos = argumentos + "d"; } else { argumentos = argumentos.Replace("d", ""); }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked) { argumentos = argumentos + "s"; } else { argumentos = argumentos.Replace("s", ""); }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked) { argumentos = argumentos + "f"; } else { argumentos = argumentos.Replace("f", ""); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //button1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }
        private void CreaRom()
        {
            try
            {
              //  if (archivo.Contains(" "))
                {
                    File.Copy(archivo, Application.StartupPath + "\\" + archivosinextension + ".DSK");
                    File.Move(Application.StartupPath + "\\" + archivosinextension + ".DSK", Application.StartupPath + "\\tmp.DSK");
                }
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.FileName = Application.StartupPath + "\\Utils\\dsk2rom.exe";
                psi.Arguments = "-" + argumentos + argumentoCompresion + " tmp.DSK " + Application.StartupPath + "\\tmp.ROM";
                var p = Process.Start(psi);
                p.WaitForExit();


            }
            catch (Exception ex) { MessageBox.Show("Ocurrio un error:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); button1.Enabled = true; return; }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            CreaRom();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\tmp.ROM"))
            {
                MessageBox.Show("No se puede convertir el archivo", "Archivo origen no valido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "ROM files (*.ROM)|*.ROM|Todos los archivos (*.*)|*.*";
                    dialog.FileName = archivosinextension + ".rom";
                    dialog.FilterIndex = 1;
                    dialog.RestoreDirectory = true;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        //  MessageBox.Show(Application.StartupPath + "\\" + archivosinextension + ".ROM\r\n"+ dialog.FileName);
                        File.Copy(Application.StartupPath + "\\tmp.ROM", dialog.FileName);
                        File.Delete(Application.StartupPath + "\\tmp.ROM");
                        File.Delete(Application.StartupPath + "\\tmp.DSK");
                    }
                    else
                    {
                        File.Delete(Application.StartupPath + "\\tmp.ROM");
                        File.Delete(Application.StartupPath + "\\tmp.DSK");
                    }
                    this.Close();
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
    }
}