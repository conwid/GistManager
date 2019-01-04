using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GistManager.Controls
{
    public class OverflowlessToolbar : ToolBar
    {
        public OverflowlessToolbar()
        {
            this.Loaded += OverflowlessToolbar_Loaded;
        }

        // https://stackoverflow.com/questions/1050953/wpf-toolbar-how-to-remove-grip-and-overflow
        private void OverflowlessToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Template.FindName("OverflowGrid", this) is FrameworkElement overflowGrid)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            if (this.Template.FindName("MainPanelBorder", this) is FrameworkElement mainPanelBorder)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }
    }
}
