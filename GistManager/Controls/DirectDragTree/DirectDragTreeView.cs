using GistManager.Utils;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GistManager.Controls.DirectDragTree
{
    public class DirectDragTreeView : TreeView
    {
        public DirectDragTreeView()
        {            
        }

        protected override DependencyObject GetContainerForItemOverride() => new DirectDragTreeViewItem();
        protected override bool IsItemItsOwnContainerOverride(object item) => item is DirectDragTreeViewItem;
    }
}

