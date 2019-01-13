using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GistManager.Behaviors
{
    public class CollectionViewSourceFilterBehavior
    {
        #region public static readonly DependencyProperty FilterCommandProperty

        public static readonly DependencyProperty FilterCommandProperty = DependencyProperty.RegisterAttached("FilterCommand", typeof(ICommand), typeof(CollectionViewSourceFilterBehavior), new PropertyMetadata(null, OnFilterCommandChanged));
        public static ICommand GetFilterCommand(DependencyObject d) => (ICommand)d.GetValue(FilterCommandProperty);
        public static void SetFilterCommand(DependencyObject d, ICommand value) => d.SetValue(FilterCommandProperty, value);

        private static void OnFilterCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CollectionViewSource collectionViewSource))
                return;

            if (e.NewValue != null)
            {
                collectionViewSource.Filter += CollectionViewSource_Filter;
            }
            else
            {
                collectionViewSource.Filter -= CollectionViewSource_Filter;
            }
        }

        private static void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var cmd = GetFilterCommand(sender as DependencyObject);
            cmd?.Execute(e);
        }
        #endregion

        #region public static readonly DependencyProperty FilterTextBoxProperty

        public static readonly DependencyProperty FilterTextBoxProperty = DependencyProperty.RegisterAttached("FilterTextBox", typeof(TextBox), typeof(CollectionViewSourceFilterBehavior), new PropertyMetadata(null, OnFilterTextBoxChanged));
        public static TextBox GetFilterTextBox(DependencyObject d) => (TextBox)d.GetValue(FilterTextBoxProperty);
        public static void SetFilterTextBox(DependencyObject d, ICommand value) => d.SetValue(FilterTextBoxProperty, value);

        private static void OnFilterTextBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CollectionViewSource collectionViewSource))
                return;

            if (e.NewValue != null)
            {
                var filterBox = (TextBox)e.NewValue;
                filterBox.TextChanged += (sender, args) => FilterBox_TextChanged(collectionViewSource, sender, args);
            }

            if (e.OldValue != null)
            {
                var filterBox = (TextBox)e.OldValue;
                filterBox.TextChanged -= (sender, args) => FilterBox_TextChanged(collectionViewSource, sender, args);
            }
        }

        private static void FilterBox_TextChanged(CollectionViewSource d, object sender, TextChangedEventArgs e) => d.View.Refresh();
        #endregion
    }
}
