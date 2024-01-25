using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace SpaceInvadersMVVM.Views;

public partial class MainWindow : Window

{
    private Canvas _gameCanvas;
    private Rectangle _spaceShip;
    private double _moveSpeed = 10.0;
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _gameCanvas = this.FindControl<Canvas>("GameCanvas");
        _spaceShip = this.FindControl<Rectangle>("SpaceShip");

        this.KeyDown += MoveSpaceShip;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MoveSpaceShip(object sender, KeyEventArgs e)
    {
        double x = Canvas.GetLeft(_spaceShip);
        double y = Canvas.GetTop(_spaceShip);

        switch (e.Key)
        {
            case Key.Left:
                if (x - _moveSpeed >= 0)
                {
                    x -= _moveSpeed;
                }
                break;
            case Key.Right:
                if (x + _moveSpeed <= _gameCanvas.Bounds.Width - _spaceShip.Width)
                {
                    x += _moveSpeed;
                }
                break;
                //case Key.Up:
                //   y -= _moveSpeed;
                //  break;
                // case Key.Down:
                //   y += _moveSpeed;
                //  break;
        }

        Canvas.SetLeft(_spaceShip, x);
        Canvas.SetTop(_spaceShip, y);
    }
}