using GistManager.Controls.DirectDragTree;
using GistManager.Mvvm.Commands.GistCommand;
using GistManager.Mvvm.Commands.RelayCommand;
using GistManager.Utils;
using GistManager.ViewModels;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace GistManager.Controls
{
    public partial class GistTreeViewDisplay : UserControl
    {
        private const string ProprietaryDragFormat = "DragSource";

        public bool IsInDarkMode { get; set; }

        //  public IsInDarkMode = Helpers.IsDarkMode();

        public GistTreeViewDisplay()
        {
            InitializeComponent();
            IsInDarkMode = Helpers.IsDarkMode();

            DataContext = this;

        }

        #region dependency properties
        public string ExpanderHeader
        {
            get => (string)GetValue(ExpanderHeaderProperty);
            set => SetValue(ExpanderHeaderProperty, value);
        }

        public static readonly DependencyProperty ExpanderHeaderProperty =
            DependencyProperty.Register(nameof(ExpanderHeader), typeof(string), typeof(GistTreeViewDisplay));

        public bool ExpanderIsEnabled
        {
            get => (bool)GetValue(ExpanderIsEnabledProperty);
            set => SetValue(ExpanderIsEnabledProperty, value);
        }

        public static readonly DependencyProperty ExpanderIsEnabledProperty =
            DependencyProperty.Register(nameof(ExpanderIsEnabled), typeof(bool), typeof(GistTreeViewDisplay));

        public bool ExpanderIsExpanded
        {
            get => (bool)GetValue(ExpanderIsExpandedProperty);
            set => SetValue(ExpanderIsExpandedProperty, value);
        }
        public static readonly DependencyProperty ExpanderIsExpandedProperty =
            DependencyProperty.Register(nameof(ExpanderIsExpanded), typeof(bool), typeof(GistTreeViewDisplay));

        public System.Collections.IEnumerable TreeViewItemsSource
        {
            get => (System.Collections.IEnumerable)GetValue(TreeViewItemSourceProperty);
            set => SetValue(TreeViewItemSourceProperty, value);
        }

        public static readonly DependencyProperty TreeViewItemSourceProperty =
            DependencyProperty.Register(nameof(TreeViewItemsSource), typeof(System.Collections.IEnumerable), typeof(GistTreeViewDisplay));

        public CreateGistCommand CreateGistCommand
        {
            get => (CreateGistCommand)GetValue(CreateGistCommandProperty);
            set => SetValue(CreateGistCommandProperty, value);
        }

        public static readonly DependencyProperty CreateGistCommandProperty =
            DependencyProperty.Register(nameof(CreateGistCommand), typeof(CreateGistCommand), typeof(GistTreeViewDisplay));


        public CreateGistFileCommand CreateGistFileCommand
        {
            get => (CreateGistFileCommand)GetValue(CreateGistFileCommandProperty);
            set => SetValue(CreateGistFileCommandProperty, value);
        }

        public static readonly DependencyProperty CreateGistFileCommandProperty =
            DependencyProperty.Register(nameof(CreateGistFileCommand), typeof(CreateGistFileCommand), typeof(GistTreeViewDisplay));


        public RelayCommand<GistViewModel> RemoveGistCommand
        {
            get => (RelayCommand<GistViewModel>)GetValue(RemoveGistCommandProperty);
            set => SetValue(RemoveGistCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveGistCommandProperty =
            DependencyProperty.Register(nameof(RemoveGistCommand), typeof(RelayCommand<GistViewModel>), typeof(GistTreeViewDisplay));



        #endregion

        #region event handlers
        private void TreeView_DragOver(object sender, DragEventArgs e) => DragInternal(e);
        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            RaiseDirectEventForCurrentTreeViewItem(DirectDragTreeViewItem.DirectDragEnterEvent, e.OriginalSource);
            DragInternal(e);
        }
        private void RaiseDirectEventForCurrentTreeViewItem(RoutedEvent routedEvent, object originalSource)
        {
            var current = (DependencyObject)originalSource;
            while (current != null && !(current is TreeViewItem))
            {
                current = VisualTreeHelper.GetParent((DependencyObject)current);
            }
            (current as TreeViewItem)?.RaiseEvent(new RoutedEventArgs(routedEvent, current));
        }
        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is TreeView tv && e.LeftButton == MouseButtonState.Pressed && tv.SelectedItem != null && tv.SelectedItem is GistFileViewModel gistFile && !gistFile.IsInEditMode)
            {
                DataObject dragDropContents = new DataObject();
                dragDropContents.SetText(gistFile.Content, TextDataFormat.UnicodeText);
                dragDropContents.SetData(ProprietaryDragFormat, this);
                DragDrop.DoDragDrop(tv, dragDropContents, DragDropEffects.Copy);
            }
        }
        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            if (CanHandleDrag(e))
            {
                e.Effects = DragDropEffects.Copy;
                string content = (string)e.Data.GetData(DataFormats.UnicodeText);
                var parent = FindUpperMostTreeViewItem((DependencyObject)e.OriginalSource);
                if (parent == null)
                {
                    CheckAndExecute(CreateGistCommand, new CreateGistCommandArgs(content));
                }
                else
                {
                    parent.IsExpanded = true;
                    var gistVm = (GistViewModel)parent.DataContext;
                    CheckAndExecute(CreateGistFileCommand, new CreateGistFileCommandArgs(content, gistVm));
                }
                e.Handled = true;
            }
        }
        private void TreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is TreeView treeView))
                return;
            HandleShortcuts(e.Key, treeView);
        }
        #endregion

        private bool CanHandleDrag(DragEventArgs e) =>
            e.Data.GetDataPresent(DataFormats.UnicodeText) && !e.Data.GetDataPresent(ProprietaryDragFormat);
        private void DragInternal(DragEventArgs e)
        {
            if (CanHandleDrag(e))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        private TreeViewItem FindTreeViewItem(DependencyObject source)
        {
            var current = source;
            while (!(current is TreeViewItem))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return (TreeViewItem)current;
        }

        private TreeViewItem FindUpperMostTreeViewItem(DependencyObject source)
        {
            TreeViewItem resultCandidate = null;
            var current = source;
            while (current != null)
            {
                if (current is TreeViewItem tv)
                {
                    resultCandidate = tv;
                }
                current = VisualTreeHelper.GetParent(current);
                if (current is TreeView)
                {
                    return resultCandidate;
                }
            }
            return null;
        }
        private void CheckAndExecute(ICommand command, object param)
        {
            if (command.CanExecute(param))
                command.Execute(param);
        }
        private void HandleShortcuts(Key key, TreeView treeView)
        {
            switch (key)
            {
                case Key.Escape:
                    RemoveItemsInEdit(treeView);
                    break;
                case Key.Delete:
                    DeleteSelectedAsync(treeView);
                    break;
                default:
                    return;
            }
        }

        private void RemoveItemsInEdit(TreeView treeView)
        {
            if (treeView.SelectedItem is CreateGistFileViewModel gistFile && gistFile.IsInEditMode)
            {
                if (gistFile.ParentGist is CreateGistViewModel createGistViewModel)
                {
                    RemoveGistCommand.Execute(gistFile.ParentGist);
                }
                else if (gistFile.ParentGist is GistViewModel)
                {
                    gistFile.ParentGist.Files.Remove(gistFile);
                }
            }
        }

        private void DeleteSelectedAsync(TreeView treeView)
        {
            if (treeView.SelectedItem is GistFileViewModel gistFile)
            {
                CheckAndExecute(gistFile.DeleteGistFileCommand, null);
            }
            else if (treeView.SelectedItem is GistViewModel gist)
            {
                CheckAndExecute(gist.DeleteGistCommand, null);
            }
        }
        private void DirectDragTreeView_DragLeave(object sender, DragEventArgs e) => RaiseDirectEventForCurrentTreeViewItem(DirectDragTreeViewItem.DirectDragLeaveEvent, e.OriginalSource);

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            // Hacky, but couldn't get to the GistItem via X:name due to it being in a DataTemplate
            if (SystemConfiguraiton.DarkModeSelected())
            {
                ((TextBlock)sender).Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            }
        }

        private void Expander_LostFocus(object sender, RoutedEventArgs e)
        {
           
        }

   
    }
}

