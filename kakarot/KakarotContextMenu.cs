using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

public class KakarotContextMenu
{
    private const string MenuName = "Lanzar con OpenMSX";
    private readonly string _pathOpenMSX;

    public KakarotContextMenu(string openMsxPath)
    {
        _pathOpenMSX = openMsxPath;
    }

    public bool IsInstalled()
    {
        return IsExtensionInstalled(".rom") && IsExtensionInstalled(".dsk");
    }

    private bool IsExtensionInstalled(string extension)
    {
        try
        {
            using (var key = Registry.ClassesRoot.OpenSubKey($"{extension}\\shell\\{MenuName}"))
            {
                return key != null;
            }
        }
        catch
        {
            return false;
        }
    }

    public bool Install()
    {
        try
        {
            // Instalar para ambas extensiones
            InstallForExtension(".rom");
            InstallForExtension(".dsk");

            // Instalación adicional específica para .dsk
            InstallDskSpecific();

            RefreshShell();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error durante la instalación: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            return false;
        }
    }

    private void InstallForExtension(string extension)
    {
        // Crear clave para la extensión
        using (var extKey = Registry.ClassesRoot.CreateSubKey(extension))
        {
            // Establecer el tipo de archivo si no existe
            if (extKey.GetValue("") == null)
            {
                extKey.SetValue("", $"{extension}file");
            }

            // Crear estructura shell > nuestro comando
            using (var shellKey = extKey.CreateSubKey("shell"))
            using (var menuKey = shellKey.CreateSubKey(MenuName))
            {
                SetMenuValues(menuKey, extension);
            }
        }
    }

    private void SetMenuValues(RegistryKey menuKey, string extension)
    {
        string iconPath = Path.Combine(_pathOpenMSX, "openmsx.exe");
        menuKey.SetValue("", "Lanzar con OpenMSX");
        menuKey.SetValue("Icon", $"\"{iconPath}\",0");

        using (var cmdKey = menuKey.CreateSubKey("command"))
        {
            string openMsxExe = Path.Combine(_pathOpenMSX, "openmsx.exe");
            string command = extension == ".rom"
                ? $"\"{openMsxExe}\" -cart \"%1\""
                : $"\"{openMsxExe}\" \"%1\"";

            cmdKey.SetValue("", command);
        }
    }

    private void InstallDskSpecific()
    {
        // Solución mejorada para .dsk: crear una asociación fuerte
        using (var dskFileKey = Registry.ClassesRoot.CreateSubKey("dskfile"))
        {
            dskFileKey.SetValue("", "MSX Disk Image");

            // Crear estructura shell para el tipo de archivo
            using (var shellKey = dskFileKey.CreateSubKey("shell"))
            using (var menuKey = shellKey.CreateSubKey(MenuName))
            {
                SetMenuValues(menuKey, ".dsk");
            }
        }

        // Asociar la extensión .dsk con nuestro tipo de archivo
        using (var dskKey = Registry.ClassesRoot.CreateSubKey(".dsk"))
        {
            dskKey.SetValue("", "dskfile");
        }
    }

    public bool Uninstall()
    {
        try
        {
            UninstallForExtension(".rom");
            UninstallForExtension(".dsk");

            // Desinstalar la solución adicional para .dsk
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree("dskfile", false);
            }
            catch { }

            RefreshShell();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error durante la desinstalación: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            return false;
        }
    }

    private void UninstallForExtension(string extension)
    {
        try
        {
            using (var extKey = Registry.ClassesRoot.OpenSubKey(extension, true))
            {
                if (extKey != null)
                {
                    try
                    {
                        extKey.DeleteSubKeyTree($"shell\\{MenuName}", false);
                    }
                    catch (ArgumentException)
                    {
                        // La clave no existe, no hay problema
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error desinstalando para {extension}: {ex.Message}");
        }
    }

    [DllImport("Shell32.dll")]
    private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

    private static void RefreshShell()
    {
        try
        {
            // Triple notificación para asegurar que Windows actualice el menú contextual
            SHChangeNotify(0x08000000, 0x1000, IntPtr.Zero, IntPtr.Zero); // SHCNE_ASSOCCHANGED
            SHChangeNotify(0x00020000, 0x1000, IntPtr.Zero, IntPtr.Zero); // SHCNE_UPDATEITEM
            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);  // SHCNE_EXTENDED_EVENT

            // Esperar un momento para que las notificaciones surtan efecto
            System.Threading.Thread.Sleep(500);
        }
        catch { }
    }

    // Nuevo método para diagnóstico del registro
    public string GetRegistryStatus()
    {
        var status = new StringBuilder();

        status.AppendLine("Estado del registro para .dsk:");
        status.AppendLine(GetKeyStatus(".dsk"));
        status.AppendLine(GetKeyStatus(".dsk\\shell\\" + MenuName));

        status.AppendLine("\nEstado del registro para dskfile:");
        status.AppendLine(GetKeyStatus("dskfile"));
        status.AppendLine(GetKeyStatus("dskfile\\shell\\" + MenuName));

        return status.ToString();
    }

    private string GetKeyStatus(string keyPath)
    {
        try
        {
            using (var key = Registry.ClassesRoot.OpenSubKey(keyPath))
            {
                if (key == null) return $"Clave no encontrada: {keyPath}";

                var value = key.GetValue("");
                var command = key.OpenSubKey("command")?.GetValue("");

                return $"Clave: {keyPath}\n" +
                       $"Valor: {(value ?? "(null)")}\n" +
                       $"Comando: {(command ?? "(no encontrado)")}";
            }
        }
        catch (Exception ex)
        {
            return $"Error accediendo a {keyPath}: {ex.Message}";
        }
    }
}