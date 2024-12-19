using kakarot;
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
    public partial class frmNombreArchivo : Form
    {
        public frmNombreArchivo()
        {
            InitializeComponent();
        }

        private void frmNombreArchivo_Load(object sender, EventArgs e)
        {
            int a = 0;
            checkBox1.Checked = false;
            string[] lines = Compartida.TodosLosArchivos.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.Replace("\r\n", "") != "") listBox1.Items.Add(line);
            }
            if (listBox1.Items.Count >1) checkBox1.Enabled = false;
            textBox1.AppendText(Compartida.NombreArchivo);
            this.Focus();
        }

        private void frmNombreArchivo_FormClosing(object sender, FormClosingEventArgs e)
        {
            Compartida.NombreArchivo = textBox1.Text;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                this.Close();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null) textBox1.Text = listBox1.SelectedItem.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\dsk2rom.rom"))
            {
                checkBox1.Checked = false;
                MessageBox.Show("Por favor, descarga el disk2rom.rom desde: http://home.kabelfoon.nl/~vincentd/ \r\n o desde https://github.com/joyrex2001/dsk2rom/ \r\n y ponlo en " + Directory.GetCurrentDirectory() + "\\","Atención");
                Process.Start("https://github.com/joyrex2001/dsk2rom");
                Process.Start("http://home.kabelfoon.nl/~vincentd/");
            }
            else
            {
                if (checkBox1.Checked) Compartida.createrom = true;
                if (!checkBox1.Checked) Compartida.createrom = false;
            }
        }

    }
}
