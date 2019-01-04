using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GistManager.Controls.DirectDragTree
{
    public class DirectDragTreeViewItem : TreeViewItem
    {
        private void DirectDragTreeViewItem_DragEnter(object sender, DragEventArgs e)
        {
            var senderTreViewItem = (TreeViewItem)sender;
            senderTreViewItem.RaiseEvent(new RoutedEventArgs(DirectDragTreeViewItem.DirectDragEnterEvent, sender));
        }

        public static readonly RoutedEvent DirectDragEnterEvent =
            EventManager.RegisterRoutedEvent(nameof(DirectDragEnter), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(DirectDragTreeViewItem));

        public event RoutedEventHandler DirectDragEnter
        {
            add => AddHandler(DirectDragEnterEvent, value);
            remove => RemoveHandler(DirectDragEnterEvent, value);
        }

        public static readonly RoutedEvent DirectDragLeaveEvent =
            EventManager.RegisterRoutedEvent(nameof(DirectDragLeave), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(DirectDragTreeViewItem));

        public event RoutedEventHandler DirectDragLeave
        {
            add => AddHandler(DirectDragLeaveEvent, value);
            remove => RemoveHandler(DirectDragLeaveEvent, value);
        }

        protected override DependencyObject GetContainerForItemOverride() => new DirectDragTreeViewItem();
        protected override bool IsItemItsOwnContainerOverride(object item) => item is DirectDragTreeViewItem;
    }
}
