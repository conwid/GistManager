using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GistManager.Behaviors
{
    public class UserControlLoadedBehavior
    {
        public static ICommand GetLoadedCommandProperty(DependencyObject obj)
                                                => (ICommand)obj.GetValue(LoadedCommandPropertyProperty);

        public static void SetLoadedCommandProperty(DependencyObject obj, ICommand value)
                                                => obj.SetValue(LoadedCommandPropertyProperty, value);


        public static readonly DependencyProperty LoadedCommandPropertyProperty =
            DependencyProperty.RegisterAttached("LoadedCommandProperty", typeof(ICommand), typeof(UserControlLoadedBehavior), new PropertyMetadata(null, OnLoadedPropertyChanged));

        private static void OnLoadedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var userControl = d as UserControl;
            if (d == null)
            {
                return;
            }
            if (e.NewValue != null)
            {                
                userControl.Loaded += WindowLoaded;
            }
            else
            {
                userControl.Loaded -= WindowLoaded;
            }
        }

        private static void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var cmd = GetLoadedCommandProperty(sender as DependencyObject);
            cmd?.Execute(e);
        }
    }
}
