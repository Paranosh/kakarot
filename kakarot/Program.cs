using System.Configuration;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace kakarot
{
    internal class Program : Form
    {
        public const string InstallArg = "/install-context-menu";
        public const string UninstallArg = "/uninstall-context-menu";

        [STAThread]
        static void Main(string[] args)
        {
            // Manejar argumentos de instalación/desinstalación
            if (args.Length > 0)
            {
                if (args[0] == InstallArg && args.Length > 1)
                {
                    string openMsxPath = args[1];
                    InstallContextMenu(openMsxPath);
                    return;
                }
                else if (args[0] == UninstallArg)
                {
                    UninstallContextMenu();
                    return;
                }
            }

            // Lógica normal de inicio de la aplicación
            bool permitirVariasInstancias;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            permitirVariasInstancias = bool.Parse(config.AppSettings.Settings["PermitirVariasInstancias"].Value);

            if (!permitirVariasInstancias)
            {
                string appName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
                if (Process.GetProcessesByName(appName).Length > 1)
                {
                    MessageBox.Show("Ya se está en ejecución, si quiere lanzar varias instancias debería de habilitarlo en la configuración",
                                   "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Environment.Exit(0);
                }
            }

            // Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            using (var splash = new Splash())
            {
                Application.Run(splash);
            }
            Application.Run(new fjntr());
        }

        private static void InstallContextMenu(string openMsxPath)
        {
            if (IsValidOpenMsxPath(openMsxPath))
            {
                var installer = new KakarotContextMenu(openMsxPath);
                bool success = installer.Install();

                if (success)
                {
                    MessageBox.Show("¡Integración con el sistema operativo completada con éxito!\n\n" +
                                   "Ahora puedes usar 'Lanzar con OpenMSX' en archivos .rom y .dsk.",
                                   "Instalación exitosa",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("La ruta de OpenMSX no es válida:\n" + openMsxPath,
                                "Error de instalación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                
            }
        }

        private static void UninstallContextMenu()
        {
            var uninstaller = new KakarotContextMenu(null);
            bool success = uninstaller.Uninstall();

            if (success)
            {
                MessageBox.Show("¡Menú contextual desinstalado con éxito!",
                                "Desinstalación exitosa",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                
            }
        }

        public static bool IsValidOpenMsxPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) &&
                   Directory.Exists(path) &&
                   File.Exists(Path.Combine(path, "openmsx.exe"));
        }
    }
}
