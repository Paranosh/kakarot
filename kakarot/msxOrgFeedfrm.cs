using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace kakarot
{
    public partial class msxOrgFeedfrm : Form
    {
        public msxOrgFeedfrm()
        {
            InitializeComponent();
        }
        List<string> noticia = new List<string>();
        List<string> urlnoticia = new List<string>();
        List<string> textonoticia = new List<string>();
        List<string> fechanoticia = new List<string>();
        private void msxOrgFeedfrm_Load(object sender, EventArgs e)
        {
            string url = "https://www.msx.org/feed/news";
            XmlReaderSettings settings = new XmlReaderSettings
            {
                // Ignorar errores de múltiples raíces (puede no funcionar en todos los casos)
                ConformanceLevel = ConformanceLevel.Fragment
            };

            using (XmlReader reader = XmlReader.Create(url, settings))
            {
                try
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    foreach (SyndicationItem item in feed.Items)
                    {
                        listBox1.Items.Add(item.Title.Text);
                        noticia.Add(item.Title.Text);
                        urlnoticia.Add(item.Links[0].Uri.ToString());
                        textonoticia.Add(item.Summary.Text);
                        listBox2.Items.Add(item.PublishDate);
                        fechanoticia.Add(item.PublishDate.ToString());
                    }
                }
                catch (XmlException ex)
                {
                    MessageBox.Show($"Error al leer XML: {ex.Message}");
                }
            }
        }
        public static String LimpiaHTML(String html)
        {
            return Regex.Replace(html, "<.*?>", String.Empty);
        }
        private void listBox1_Click(object sender, EventArgs e)
        {
            foreach (var item in noticia)
            {
                if (item == listBox1.SelectedItem.ToString())
                {
                    int index = noticia.IndexOf(item);
                    linkLabel1.Text = urlnoticia[index];
                    textBox1.Text = LimpiaHTML(textonoticia[index]).Trim() + "....";
                    listBox2.SelectionMode = SelectionMode.One;
                    listBox2.SelectedIndex = index;

                    break;
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = linkLabel1.Text,
                UseShellExecute = true
            });
        }

        private void listBox2_Click(object sender, EventArgs e)
        {
            listBox2.SelectionMode = SelectionMode.None;
            listBox2.SelectionMode = SelectionMode.One;
            listBox2.SelectedIndex = listBox1.SelectedIndex;
        }
    }
}
