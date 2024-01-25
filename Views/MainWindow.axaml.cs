using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;

namespace SpaceInvadersMVVM.Views;

public partial class MainWindow : Window

{
    private Canvas _gameCanvas;
    private Rectangle _spaceShip;
    private double _moveSpeed = 10.0;
    private List<Rectangle> _enemies;
    private DispatcherTimer _timer;
    private double _invadersDirection = 1; // 1 para direita, -1 para esquerda

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _gameCanvas = this.FindControl<Canvas>("GameCanvas");
        _spaceShip = this.FindControl<Rectangle>("SpaceShip");

        _enemies = new List<Rectangle>();

        int rows = 5;
        int cols = 11;
        double enemyWidth = 50;
        double enemyHeight = 50;
        double enemyMargin = 10;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var enemy = new Rectangle
                {
                    Width = enemyWidth,
                    Height = enemyHeight,
                    Fill = Brushes.Red
                };

                Canvas.SetLeft(enemy, col * (enemyWidth + enemyMargin));
                Canvas.SetTop(enemy, row * (enemyHeight + enemyMargin));

                _gameCanvas.Children.Add(enemy);
                _enemies.Add(enemy);
            }
        }

        this.KeyDown += MoveSpaceShip;

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(100);
        _timer.Tick += MoveEnemies;
        _timer.Start();
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
            case Key.Left or Key.A:
                if (x - _moveSpeed >= 0)
                {
                    x -= _moveSpeed;
                }
                break;
            case Key.Right or Key.D:
                if (x + _moveSpeed <= _gameCanvas.Bounds.Width - _spaceShip.Width)
                {
                    x += _moveSpeed;
                }
                break;
           
        }

        Canvas.SetLeft(_spaceShip, x);
        Canvas.SetTop(_spaceShip, y);
    }
    private void MoveEnemies(object sender, EventArgs e)
    {
        bool shouldMoveDown = false;

        foreach (var enemy in _enemies)
        {
            double x = Canvas.GetLeft(enemy);
            double moveSpeed = _moveSpeed * _invadersDirection;

            if (x + moveSpeed < 0 || x + moveSpeed > _gameCanvas.Bounds.Width - enemy.Width)
            {
                _invadersDirection *= -1;
                shouldMoveDown = true;
                break;
            }
        }

        foreach (var enemy in _enemies)
        {
            double x = Canvas.GetLeft(enemy);
            double y = Canvas.GetTop(enemy);
            double moveSpeed = _moveSpeed * _invadersDirection;

            Canvas.SetLeft(enemy, x + moveSpeed);

            if (shouldMoveDown)
            {
                Canvas.SetTop(enemy, y + 20);
            }
        }
    }



}