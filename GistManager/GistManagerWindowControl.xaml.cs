namespace GistManager
{
    using GistManager.Behaviors;
    using GistManager.GistService;
    using GistManager.GistService.Wpf;
    using GistManager.ViewModels;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class GistManagerWindowControl : UserControl
    {
        private readonly GistManagerWindowViewModel viewModel;
        public GistManagerWindowControl(GistManagerWindowViewModel gistManagerWindowViewModel)
        {
            InitializeComponent();
            viewModel = gistManagerWindowViewModel;
            DataContext = viewModel;
        }        
    }
}