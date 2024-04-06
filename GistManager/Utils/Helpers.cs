using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace GistManager.Utils
{
    internal static class Helpers
    {
        internal static T FindParentOfType<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentDepObj = child;
            do
            {
                parentDepObj = VisualTreeHelper.GetParent(parentDepObj);
                T parent = parentDepObj as T;
                if (parent != null) return parent;
            }
            while (parentDepObj != null);
            return null;
        }

        /// <summary>
        /// Sure there's a better way to do this, but meh
        /// Examines VS's ToolWindowBackgroundColorKey. If Red lower than 100 (arbitrary) return dark mode is true
        /// Red due to BGs RGB values all being same(ish?)
        /// </summary>
        /// <returns></returns>
        internal static bool IsDarkMode()
        {
            var defaultBackground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
            if (defaultBackground.R < 100) return true;
            return false;
        }

    }
}
