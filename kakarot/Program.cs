using System.Configuration;

namespace kakarot
{
    internal class Program : Form
    {


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool permitirVariasInstancias;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            permitirVariasInstancias = bool.Parse(config.AppSettings.Settings["PermitirVariasInstancias"].Value);
            if (!permitirVariasInstancias)
            {
                if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
                {
                    MessageBox.Show("Ya se está en ejecución, si quiere lanzar varias instancias deberia de habilitarlo en la configuración", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Environment.Exit(0);
                }
            }
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            using (var splash = new Splash())
            {
                Application.Run(splash);
            }
            Application.Run(new fjntr());
        }
    }
}