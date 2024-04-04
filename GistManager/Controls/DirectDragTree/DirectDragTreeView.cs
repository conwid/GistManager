using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GistManager.Controls.DirectDragTree
{
    public class DirectDragTreeView : TreeView
    {       
        protected override DependencyObject GetContainerForItemOverride() => new DirectDragTreeViewItem();
        protected override bool IsItemItsOwnContainerOverride(object item) => item is DirectDragTreeViewItem;
    }
}

