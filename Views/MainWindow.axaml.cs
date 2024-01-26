using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using SpaceInvaders.Models;

namespace SpaceInvadersMVVM.Views;

public partial class MainWindow : Window

{
    private Canvas _gameCanvas;
    private Image _spaceShip;
    private Player _player;
    private Image _enemy;
    private Image _bullet;
    private double _moveSpeed = 2.0;
    private double _playerSpeed = 5.0;
    private List<Image> _enemies;
    private List<Image> _bullets;
    private DispatcherTimer _timer;
    private double _invadersDirection = 1; // 1 para direita, -1 para esquerda

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _gameCanvas = this.FindControl<Canvas>("GameCanvas");
        _spaceShip = this.FindControl<Image>("SpaceShip");
        _enemy = this.FindControl<Image>("enemy");
        _bullet = this.FindControl<Image>("bala");
        _enemies = new List<Image>();
        _bullets = new List<Image>();
        _player = new Player();

        int rows = 5;
        int cols = 11;
        double enemyWidth = 40;
        double enemyHeight = 40;
        double enemyMargin = 10;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var enemy = new Image
                {
                    Width = enemyWidth,
                    Height = enemyHeight,
                    Source = _enemy.Source,  // Clonando a propriedade Source do inimigo original
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
        _player.X = Canvas.GetLeft(_spaceShip);
        _player.Y = Canvas.GetLeft(_spaceShip);

        switch (e.Key)
        {
            case Key.Left or Key.A:
                if (_player.X - _playerSpeed >= 0)
                {
                    _player.X -= _playerSpeed;
                }
                break;
            case Key.Right or Key.D:
                if (_player.X + _playerSpeed <= _gameCanvas.Bounds.Width - _spaceShip.Width)
                {
                    _player.X += _playerSpeed;
                }
                break; 
            case Key.Space:
                double bulletX = _player.X + _spaceShip.Width / 2 - 2.5; // Centralizar a bala em relação à nave espacial
                double bulletY = _player.Y;
                CreateBullet(bulletX, bulletY, 2, true); // Criar bala do jogador
                break;
        }

        Canvas.SetLeft(_spaceShip, _player.X);
        Canvas.SetTop(_spaceShip, _player.Y);
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


    
    private void CreateBullet(double x, double y, double speed, bool isPlayerBullet)
    {
        var bullet = new Image
        {
            Width = 20,
            Height = 35,
            Source = _bullet.Source, // Substitua com o caminho real da imagem da bala
        };

        Canvas.SetLeft(bullet, x);
        Canvas.SetTop(bullet, y);

        _gameCanvas.Children.Add(bullet);
        _bullets.Add(bullet);

        // Iniciar o movimento da bala
        DispatcherTimer bulletTimer = new DispatcherTimer();
        bulletTimer.Interval = TimeSpan.FromMilliseconds(20);
        bulletTimer.Tick += (sender, e) =>
        {
            double bulletY = Canvas.GetTop(bullet);

            if (isPlayerBullet)
            {   
                bulletY -= speed;
            }
            else
            {
                bulletY += speed;
            }
            Canvas.SetTop(bullet, bulletY);


            // Verificar colisão com inimigos ou jogador
            CheckBulletCollision(bullet, isPlayerBullet, bulletTimer);

            // Remover a bala se estiver fora da tela
            if (bulletY < 0 || bulletY > _gameCanvas.Bounds.Height)
            {
                _gameCanvas.Children.Remove(bullet);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Parar o timer da bala
            }
        };

        bulletTimer.Start();
    }

    private void CheckBulletCollision(Image bullet, bool isPlayerBullet, DispatcherTimer bulletTimer)
    {
        if (isPlayerBullet)
        {
            // Verificar colisão com inimigos
            foreach (var enemy in _enemies)
            {
                if (CheckCollision(bullet, enemy))
                {
                    // Remover inimigo e bala
                    _gameCanvas.Children.Remove(enemy);
                    _enemies.Remove(enemy);
                    _gameCanvas.Children.Remove(bullet);
                    _bullets.Remove(bullet);
                    bulletTimer.Stop(); // Parar o timer da bala
                    break;
                }
            }
        }
        else
        {
            // Verificar colisão com jogador
            if (CheckCollision(bullet, _spaceShip))
            {
                // Lidar com a colisão com o jogador (por exemplo, perder vida)
                _gameCanvas.Children.Remove(bullet);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Parar o timer da bala
            }
        }

        // Remover bala se estiver fora da tela
        if (Canvas.GetTop(bullet) < 0 || Canvas.GetTop(bullet) > _gameCanvas.Bounds.Height)
        {
            _gameCanvas.Children.Remove(bullet);
            _bullets.Remove(bullet);
            bulletTimer.Stop(); // Parar o timer da bala
        }
    }
    
    private bool CheckCollision(Image element1, Image element2)
    {
        Rect rect1 = new Rect(Canvas.GetLeft(element1), Canvas.GetTop(element1), element1.Width, element1.Height);
        Rect rect2 = new Rect(Canvas.GetLeft(element2), Canvas.GetTop(element2), element2.Width, element2.Height);

        return rect1.Intersects(rect2);
    }
    


}