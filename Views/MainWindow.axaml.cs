using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using SpaceInvadersMVVM.Models;
using SpaceInvadersMVVM.ViewModels;
// using NAudio.Wave;


namespace SpaceInvadersMVVM.Views;

public partial class MainWindow : Window

{
    private readonly Canvas? _gameCanvas;
    private Image? _spaceShip;
    private Player _player;
    private bool _canShoot = true;
    private double _moveSpeed = 2.0;
    private double _playerSpeed = 5.0;
    private List<Invader> _enemies;
    private List<Barrier> _shields;
    private List<Image> _bullets;
    private DispatcherTimer _timer;
    private double _invadersDirection = 1; // 1 para direita, -1 para esquerda
    private DispatcherTimer _enemyBulletTimer;
    // private WaveOutEvent _waveOut;
    // private WaveFileReader _explosion;

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _gameCanvas = this.FindControl<Canvas>("GameCanvas");
        _spaceShip = this.FindControl<Image>("SpaceShip");
        _enemies = new List<Invader>();
        _bullets = new List<Image>();
        _shields = new List<Barrier>();
        _player = new Player();
        // _waveOut = new WaveOutEvent();
        // _explosion = new WaveFileReader("Assets/2.wav");
        _enemyBulletTimer = new DispatcherTimer();
        _enemyBulletTimer.Interval = TimeSpan.FromMilliseconds(1500); // Defina o intervalo desejado para o tiro dos inimigos
        _enemyBulletTimer.Tick += EnemyShoot!;
        _enemyBulletTimer.Start();

        const int numShields = 4;
        const double shieldMargin = 80;

        for (var i = 0; i < numShields; i++)
        {
            var barrier = new Barrier();
            barrier.Sprite!.Source = this.FindControl<Image>("Shield")?.Source;

            Canvas.SetLeft(barrier.Sprite, i * (800.00 / numShields) + shieldMargin);
            Canvas.SetTop(barrier.Sprite, 600 - barrier.Sprite.Height - 100);

            _gameCanvas!.Children.Add(barrier.Sprite);
            _shields.Add(barrier);
        }
        
        const int rows = 5;
        const int cols = 11;
        double enemyMargin = 10;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {

                var enemy = new Invader(col, row);

                switch (row)
                {
                    case 0:
                        enemy.Sprite!.Source = this.FindControl<Image>("Enemy1")?.Source;
                        break;
                    case 1:
                    case 2:
                        enemy.Sprite!.Source = this.FindControl<Image>("Enemy2")?.Source;
                        break;
                    case 3:
                    case 4:
                        enemy.Sprite!.Source = this.FindControl<Image>("Enemy3")?.Source;
                        break;
                    default:
                        enemy.Sprite!.Source = this.FindControl<Image>("Enemy1")?.Source;
                        break;
                }

                Canvas.SetLeft(enemy.Sprite, col * (enemy.Sprite.Width + enemyMargin));
                Canvas.SetTop(enemy.Sprite, row * (enemy.Sprite.Height + enemyMargin));

                _gameCanvas?.Children.Add(enemy.Sprite);
                _enemies.Add(enemy);
            }
        }
        KeyDown += MoveSpaceShip;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _timer.Tick += MoveEnemies;
        _timer.Start();

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MoveSpaceShip(object? sender, KeyEventArgs e)
    {
        _player.X = Canvas.GetLeft(_spaceShip!);

        switch (e.Key)
        {
            case Key.Left or Key.A:
                if (_player.X - _playerSpeed >= 0)
                {
                    _player.X -= _playerSpeed;
                }
                break;
            case Key.Right or Key.D:
                if (_player.X + _playerSpeed <= _gameCanvas!.Bounds.Width - _spaceShip!.Width)
                {
                    _player.X += _playerSpeed;
                }
                break;
            case Key.Space:
                if (_canShoot)
                {
                    var bulletX = _player.X + (_spaceShip!.Width / 2) - 10;
                    var bulletY = _player.Y;
                    CreateBullet(bulletX, bulletY, 3, true);
                    _canShoot = false; // Impede que o jogador atire novamente imediatamente
                }
                break;
        }

        Canvas.SetLeft(_spaceShip!, _player.X);
        Canvas.SetTop(_spaceShip!, _player.Y);
    }

    private void MoveEnemies(object? sender, EventArgs e)
    {
        var shouldMoveDown = false;

        foreach (var enemy in _enemies)
        {
            var x = Canvas.GetLeft(enemy.Sprite!);
            var moveSpeed = _moveSpeed * _invadersDirection;

            if (x + moveSpeed < 0 || x + moveSpeed > _gameCanvas!.Bounds.Width - enemy.Sprite!.Width)
            {
                _invadersDirection *= -1;
                shouldMoveDown = true;
                break;
            }
        }

        foreach (var enemy in _enemies)
        {
            var x = Canvas.GetLeft(enemy.Sprite!);
            var y = Canvas.GetTop(enemy.Sprite!);
            var moveSpeed = _moveSpeed * _invadersDirection;

            Canvas.SetLeft(enemy.Sprite!, x + moveSpeed);

            switch (enemy.Row)
            {
                case 0:
                    enemy.Sprite!.Source = (enemy.Sprite.Source == this.FindControl<Image>("Enemy1")?.Source)
                        ? this.FindControl<Image>("Enemy1A")?.Source
                        : this.FindControl<Image>("Enemy1")?.Source;
                    break;
                case 1:
                case 2:
                    enemy.Sprite!.Source = (enemy.Sprite.Source == this.FindControl<Image>("Enemy2")?.Source)
                        ? this.FindControl<Image>("Enemy2A")?.Source
                        : this.FindControl<Image>("Enemy2")?.Source;
                    break;
                default:
                    enemy.Sprite!.Source = (enemy.Sprite.Source == this.FindControl<Image>("Enemy3")?.Source)
                        ? this.FindControl<Image>("Enemy3A")?.Source
                        : this.FindControl<Image>("Enemy3")?.Source;
                    break;
            }

            if (shouldMoveDown)
            {
                Canvas.SetTop(enemy.Sprite, y + 20);
            }
        }
    }



    private void CreateBullet(double x, double y, double speed, bool isPlayerBullet)
    {
        var bullet = new Image
        {
            Width = 20,
            Height = 35,
            Source = this.FindControl<Image>("Bala")?.Source,
        };
        
        if(!isPlayerBullet){
            bullet.RenderTransform = new RotateTransform(180);
        }

        Canvas.SetLeft(bullet, x);
        Canvas.SetTop(bullet, y);

        _gameCanvas!.Children.Add(bullet);
        _bullets.Add(bullet);

        // Iniciar o movimento da bala
        var bulletTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(20)
        };
        bulletTimer.Tick += (_, _) =>
        {
            var bulletY = Canvas.GetTop(bullet);

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

            // Remove a bala se estiver fora da tela
            if (bulletY < 0 || bulletY > _gameCanvas.Bounds.Height)
            {
                _gameCanvas.Children.Remove(bullet);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Parar o timer da bala
                if (isPlayerBullet)
                {
                    _canShoot = true;
                }
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
                if (CheckCollision(bullet, enemy.Sprite!))
                {
                    // Remover inimigo e bala
                    // _waveOut.Init(_explosion);
                    // _waveOut.Play();

                    _gameCanvas!.Children.Remove(enemy.Sprite!);
                    _enemies.Remove(enemy);
                    _gameCanvas.Children.Remove(bullet);
                    _bullets.Remove(bullet);
                    bulletTimer.Stop(); // Parar o timer da bala
                    _canShoot = true;
                    ((MainWindowViewModel)DataContext!).UpdateScore(10);
                    break;
                }
            }
        }
        else
        {
            // Verificar colisão com jogador
            if (CheckCollision(bullet, _spaceShip!))
            {
                // Lidar com a colisão com o jogador (por exemplo, perder vida)
                _gameCanvas!.Children.Remove(bullet);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Parar o timer da bala
                _canShoot = true;
            }
        }
        
        // Verificar colisão com barreiras
        foreach (var shield in _shields)
        {
            if (CheckCollision(bullet, shield.Sprite!))
            {
                // Remover a bala e danificar a barreira
                _gameCanvas!.Children.Remove(bullet);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Parar o timer da bala
                _canShoot = true;
                switch (shield.Life)
                {
                    case > 80:
                        shield.Sprite!.Source = this.FindControl<Image>("Shield1")?.Source;
                        break;
                    case > 60:
                        shield.Sprite!.Source = this.FindControl<Image>("Shield2")?.Source;
                        break;
                    case > 40:
                        shield.Sprite!.Source = this.FindControl<Image>("Shield3")?.Source;
                        break;
                    case > 20:
                        shield.Sprite!.Source = this.FindControl<Image>("Shield4")?.Source;
                        break;
                    default:
                        _gameCanvas.Children.Remove(shield.Sprite!);
                        _shields.Remove(shield);
                        return; // Saia do switch quando o escudo for removido
                }

                shield.Life -= 20;

                break;
            }
        }

        // Remover bala se estiver fora da tela
        if (Canvas.GetTop(bullet) < 0 || Canvas.GetTop(bullet) > _gameCanvas!.Bounds.Height)
        {
            _gameCanvas!.Children.Remove(bullet);
            _bullets.Remove(bullet);
            bulletTimer.Stop(); // Parar o timer da bala
            _canShoot = true;
        }
    }

    private bool CheckCollision(Image element1, Image element2)
    {
        var rect1 = new Rect(Canvas.GetLeft(element1), Canvas.GetTop(element1), element1.Width, element1.Height);
        var rect2 = new Rect(Canvas.GetLeft(element2), Canvas.GetTop(element2), element2.Width, element2.Height);

        return rect1.Intersects(rect2);
    }

    private void EnemyShoot(object sender, EventArgs e)
    {
        var random = new Random();

        // Escolha um inimigo aleatório para atirar
        if (_enemies.Count > 0)
        {
            var randomEnemyIndex = random.Next(_enemies.Count);
            var randomEnemy = _enemies[randomEnemyIndex].Sprite;

            var bulletX = Canvas.GetLeft(randomEnemy!) + (randomEnemy!.Width / 2) - 10;
            var bulletY = Canvas.GetTop(randomEnemy) + randomEnemy.Height;

            CreateBullet(bulletX, bulletY, 3, false);
            CheckBulletCollision(_bullets.Last(), false, _enemyBulletTimer);
        }
    }


}