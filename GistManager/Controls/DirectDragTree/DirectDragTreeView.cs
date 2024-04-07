using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GistManager.Controls.DirectDragTree
{
    public class DirectDragTreeView : TreeView
    {
        public DirectDragTreeView()
        {
            this.MouseDoubleClick += DirectDragTreeView_MouseDoubleClick
                ;

        }

        private void DirectDragTreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("TreeViewItem Double Click");
            //e.can
            //throw new NotImplementedException();
        }

        protected override DependencyObject GetContainerForItemOverride() => new DirectDragTreeViewItem();
        protected override bool IsItemItsOwnContainerOverride(object item) => item is DirectDragTreeViewItem;
    }
}

