using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Data;
using SpaceInvadersMVVM.ViewModels;

namespace SpaceInvadersMVVM.Views
{
   
    public partial class StartScreen : UserControl
    {
        private readonly MainWindowViewModel _viewModel;

        public StartScreen(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            // Criar uma instância de MainWindowViewModel e carregar os scores do arquivo CSV
            _viewModel = viewModel;

            // Definir o DataContext da StartScreen como a instância de MainWindowViewModel
            DataContext = _viewModel;

            // Configurar as colunas do DataGrid manualmente
            var dataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Avalonia.Thickness(10),
                Width = 400
            };
            
            // Definir o ItemsSource do DataGrid como a propriedade Scores do DataContext
            dataGrid.ItemsSource = _viewModel.Scores;
            
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Nickname",
                Binding = new Binding("Nickname")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Score",
                Binding = new Binding("ScoreValue")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Date",
                Binding = new Binding("Date")
            });
            

            // Adicionar o DataGrid ao StackPanel
            var stackPanel = new StackPanel
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            stackPanel.Children.Add(new TextBlock
            {
                Text = "Pressione enter para iniciar o jogo",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                FontSize = 24
            });
            stackPanel.Children.Add(dataGrid);
            Content = stackPanel;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

}