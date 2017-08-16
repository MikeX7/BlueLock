using System.Windows.Forms;
using Microsoft.Win32;

namespace BlueLock
{
    public static class BootStarter
    {
        public static void ToggleAutostart(bool state)
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (state)
                rk.SetValue(Application.ProductName, Application.ExecutablePath.ToString());
            else
                rk.DeleteValue(Application.ProductName, false);

        }
    }
}
