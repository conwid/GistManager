using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Utils
{
    internal static class SystemConfiguraiton
    {
        internal static bool DarkModeSelected()
        {
            // Legacy - theme changed according to Global windows dark mode (not App dark mode)
            //int res = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "SystemUsesLightTheme", -1);
            //if (res == 0) return true;
            //return false;

            return Properties.Settings.Default.DarkMode;
        }
    }
}
