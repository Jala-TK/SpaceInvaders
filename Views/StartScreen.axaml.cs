using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SpaceInvadersMVVM.ViewModels;

namespace SpaceInvadersMVVM.Views
{
    public partial class StartScreen : UserControl
    {
        private MainWindowViewModel _viewModel;
        public event EventHandler StartGameClicked;

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
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGameClicked?.Invoke(this, EventArgs.Empty);
        }


    }
}