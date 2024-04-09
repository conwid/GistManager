using Octokit;
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

        public DirectDragTreeViewItem()
        {
            this.Loaded += DirectDragTreeViewItem_Loaded; 
        }

        private void DirectDragTreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            // The purpose of this code is to stretch the Header Content all the way accross the TreeView. 
            if (this.VisualChildrenCount > 0)
            {
                Grid grid = this.GetVisualChild(0) as Grid;
                if (grid != null && grid.ColumnDefinitions.Count == 3)
                {
                    // Remove the middle column which is set to Auto and let it get replaced with the 
                    // last column that is set to Star.
                    //grid.ColumnDefinitions.RemoveAt(1);

                    //grid.ColumnDefinitions[1].Width = new GridLength(0);
                }
            }
            //if (this.VisualChildrenCount > 0)
            //{
            //    Grid grid = this.GetVisualChild(0) as Grid;
            //    double gridWidth = grid.ActualWidth;

            //    if (grid.ColumnDefinitions.Count > 1)
            //    {
            //        var lastColumnGridLength = grid.ColumnDefinitions.Last();

            //        double resolvedWidth = grid.ColumnDefinitions.Last().ActualWidth + grid.ColumnDefinitions[grid.ColumnDefinitions.Count -1].ActualWidth;
            //        grid.ColumnDefinitions.Remove(grid.ColumnDefinitions[grid.ColumnDefinitions.Count - 1]);
            //        grid.ColumnDefinitions.Last().Width = new GridLength(resolvedWidth);
            //    }  
            //}

        }

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
