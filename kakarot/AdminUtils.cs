using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

public static class AdminUtils
{
    public static bool IsRunningAsAdmin()
    {
        using (var identity = WindowsIdentity.GetCurrent())
        {
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    public static bool RestartAsAdmin(string arguments = "")
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                Arguments = arguments,
                UseShellExecute = true,
                Verb = "runas"
            };

            Process.Start(startInfo);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}