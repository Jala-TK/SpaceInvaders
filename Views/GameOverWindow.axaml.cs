using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpaceInvadersMVVM.ViewModels;

namespace SpaceInvadersMVVM.Views
{
    public partial class GameOverWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public GameOverWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
            this.FindControl<TextBlock>("Score")!.Text = _viewModel.Score;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
