using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpaceInvadersMVVM.ViewModels;

namespace SpaceInvadersMVVM.Views
{
    public partial class StartScreen : UserControl
    {
        public StartScreen(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}