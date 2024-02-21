using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpaceInvadersMVVM.ViewModels;

namespace SpaceInvadersMVVM.Views
{
    public partial class StartScreen : UserControl
    {
        private readonly MainWindowViewModel _viewModel;

        public StartScreen(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}