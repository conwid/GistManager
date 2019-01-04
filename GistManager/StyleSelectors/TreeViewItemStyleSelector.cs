using GistManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GistManager.StyleSelectors
{
    public class TreeViewItemStyleSelector : StyleSelector
    {
        private const string gistStyleKey = "gistStyle";
        private const string gistFileStyleKey = "gistFileStyle";

        public override Style SelectStyle(object item, DependencyObject container)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(container);
            while (!(parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            var treeViewParent = (TreeView)parent;
            if (item is GistViewModel)
                return (Style)treeViewParent.Resources[gistStyleKey];
            if (item is GistFileViewModel)
                return (Style)treeViewParent.Resources[gistFileStyleKey];
            throw new NotSupportedException($"Unknown item in treeview: {item.GetType()}");
        }
    }
}
