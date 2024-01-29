using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using SpaceInvaders.Models;
using SpaceInvadersMVVM.ViewModels;
using NAudio.Wave;


namespace SpaceInvadersMVVM.Views;

public partial class MainWindow : Window

{
    private Canvas _gameCanvas;
    private Image _spaceShip;
    private Player _player;
    private Image _enemy1;
    private Image _enemy1a;
    private Image _enemy2;
    private Image _enemy2a;
    private Image _enemy3;
    private Image _enemy3a;
    private Image _bullet;
    private Image _shield;
    private Image _shield1;
    private Image _shield2;
    private Image _shield3;
    private Image _shield4;
    private bool canShoot = true;
    private double _moveSpeed = 2.0;
    private double _playerSpeed = 5.0;
    private List<Invader> _enemies;
    private List<Shield> _shields;
    private List<Image> _bullets;
    private DispatcherTimer _timer;
    private double _invadersDirection = 1; // 1 para direita, -1 para esquerda
    private DispatcherTimer _enemyBulletTimer;
    private WaveOutEvent _waveOut;
    private WaveFileReader _explosion;

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _gameCanvas = this.FindControl<Canvas>("GameCanvas");
        _spaceShip = this.FindControl<Image>("SpaceShip");
        _enemy1 = this.FindControl<Image>("enemy1");
        _enemy1a = this.FindControl<Image>("enemy1a");
        _enemy2 = this.FindControl<Image>("enemy2");
        _enemy2a = this.FindControl<Image>("enemy2a");
        _enemy3 = this.FindControl<Image>("enemy3");
        _enemy3a = this.FindControl<Image>("enemy3a");
        _bullet = this.FindControl<Image>("bala");
        _shield = this.FindControl<Image>("shield");
        _shield1 = this.FindControl<Image>("shield1");
        _shield2 = this.FindControl<Image>("shield2");
        _shield3 = this.FindControl<Image>("shield3");
        _shield4 = this.FindControl<Image>("shield4");
        _enemies = new List<Invader>();
        _bullets = new List<Image>();
        _shields = new List<Shield>();
        _player = new Player();
        _waveOut = new WaveOutEvent();
        // _explosion = new WaveFileReader("Assets/2.wav");
        _enemyBulletTimer = new DispatcherTimer();
        _enemyBulletTimer.Interval = TimeSpan.FromMilliseconds(1500); // Defina o intervalo desejado para o tiro dos inimigos
        _enemyBulletTimer.Tick += EnemyShoot;
        _enemyBulletTimer.Start();

        int numShields = 4;
        double shieldMargin = 80;

        for (int i = 0; i < numShields; i++)
        {
            var barrer = new Shield();
            barrer.Sprite.Source = _shield.Source;

            Canvas.SetLeft(barrer.Sprite, i * (800 / numShields) + shieldMargin);
            Canvas.SetTop(barrer.Sprite, 600 - barrer.Sprite.Height - 100);

            _gameCanvas.Children.Add(barrer.Sprite);
            _shields.Add(barrer);
        }
        
        int rows = 5;
        int cols = 11;
        double enemyMargin = 10;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {

                var enemy = new Invader(col, row);

                switch (row)
                {
                    case 0:
                        enemy.Sprite.Source = _enemy1?.Source;
                        break;
                    case 1:
                    case 2:
                        enemy.Sprite.Source = _enemy2?.Source;
                        break;
                    case 3:
                    case 4:
                        enemy.Sprite.Source = _enemy3?.Source;
                        break;
                    default:
                        enemy.Sprite.Source = _enemy1?.Source;
                        break;
                }

                Canvas.SetLeft(enemy.Sprite, col * (enemy.Sprite.Width + enemyMargin));
                Canvas.SetTop(enemy.Sprite, row * (enemy.Sprite.Height + enemyMargin));

                _gameCanvas?.Children.Add(enemy.Sprite);
                _enemies.Add(enemy);
            }
        }
        KeyDown += MoveSpaceShip;

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(100);
        _timer.Tick += MoveEnemies;
        _timer.Start();

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MoveSpaceShip(object? sender, KeyEventArgs e)
    {
        _player.X = Canvas.GetLeft(_spaceShip);

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
                if (canShoot)
                {
                    double bulletX = _player.X + (_spaceShip.Width / 2) - 10;
                    double bulletY = _player.Y;
                    CreateBullet(bulletX, bulletY, 3, true);
                    canShoot = false; // Impede que o jogador atire novamente imediatamente
                }
                break;
        }

        Canvas.SetLeft(_spaceShip, _player.X);
        Canvas.SetTop(_spaceShip, _player.Y);
    }

    private void MoveEnemies(object? sender, EventArgs e)
    {
        bool shouldMoveDown = false;

        foreach (var enemy in _enemies)
        {
            double x = Canvas.GetLeft(enemy.Sprite);
            double moveSpeed = _moveSpeed * _invadersDirection;

            if (x + moveSpeed < 0 || x + moveSpeed > _gameCanvas.Bounds.Width - enemy.Sprite.Width)
            {
                _invadersDirection *= -1;
                shouldMoveDown = true;
                break;
            }
        }

        foreach (var enemy in _enemies)
        {
            double x = Canvas.GetLeft(enemy.Sprite);
            double y = Canvas.GetTop(enemy.Sprite);
            double moveSpeed = _moveSpeed * _invadersDirection;

            Canvas.SetLeft(enemy.Sprite, x + moveSpeed);

            if (enemy.Row == 0)
            {
                enemy.Sprite.Source = (enemy.Sprite.Source == _enemy1.Source) ? _enemy1a.Source : _enemy1.Source;

            }else if(enemy.Row == 1 || enemy.Row == 2)
            {
                enemy.Sprite.Source = (enemy.Sprite.Source == _enemy2.Source) ? _enemy2a.Source : _enemy2.Source;

            }
            else
            {
                enemy.Sprite.Source = (enemy.Sprite.Source == _enemy3.Source) ? _enemy3a.Source : _enemy3.Source;

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
            Source = _bullet.Source,
        };
        
        if(!isPlayerBullet){
            bullet.RenderTransform = new RotateTransform(180);
        }

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

            // Remove a bala se estiver fora da tela
            if (bulletY < 0 || bulletY > _gameCanvas.Bounds.Height)
            {
                _gameCanvas.Children.Remove(bullet);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Parar o timer da bala
                canShoot = true;
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
                if (CheckCollision(bullet, enemy.Sprite))
                {
                    // Remover inimigo e bala
                    // _waveOut.Init(_explosion);
                    // _waveOut.Play();

                    _gameCanvas.Children.Remove(enemy.Sprite);
                    _enemies.Remove(enemy);
                    _gameCanvas.Children.Remove(bullet);
                    _bullets.Remove(bullet);
                    bulletTimer.Stop(); // Parar o timer da bala
                    canShoot = true;
                    ((MainWindowViewModel)this.DataContext).UpdateScore(10);
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
                canShoot = true;
            }
        }
        
        // Verificar colisão com barreiras
        foreach (var shield in _shields)
        {
            if (CheckCollision(bullet, shield.Sprite))
            {
                // Remover a bala e danificar a barreira
                _gameCanvas.Children.Remove(bullet);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Parar o timer da bala
                canShoot = true;
                switch (shield.Life)
                {
                    case > 80:
                        shield.Sprite.Source = _shield1.Source;
                        break;
                    case > 60:
                        shield.Sprite.Source = _shield2.Source;
                        break;
                    case > 40:
                        shield.Sprite.Source = _shield3.Source;
                        break;
                    case > 20:
                        shield.Sprite.Source = _shield4.Source;
                        break;
                    default:
                        _gameCanvas.Children.Remove(shield.Sprite);
                        _shields.Remove(shield);
                        return; // Saia do switch quando o escudo for removido
                }

                shield.Life -= 20;

                break;
            }
        }

        // Remover bala se estiver fora da tela
        if (Canvas.GetTop(bullet) < 0 || Canvas.GetTop(bullet) > _gameCanvas.Bounds.Height)
        {
            _gameCanvas.Children.Remove(bullet);
            _bullets.Remove(bullet);
            bulletTimer.Stop(); // Parar o timer da bala
            canShoot = true;
        }
    }

    private bool CheckCollision(Image element1, Image element2)
    {
        Rect rect1 = new Rect(Canvas.GetLeft(element1), Canvas.GetTop(element1), element1.Width, element1.Height);
        Rect rect2 = new Rect(Canvas.GetLeft(element2), Canvas.GetTop(element2), element2.Width, element2.Height);

        return rect1.Intersects(rect2);
    }

    private void EnemyShoot(object sender, EventArgs e)
    {
        Random random = new Random();

        // Escolha um inimigo aleatório para atirar
        if (_enemies.Count > 0)
        {
            int randomEnemyIndex = random.Next(_enemies.Count);
            Image randomEnemy = _enemies[randomEnemyIndex].Sprite;

            double bulletX = Canvas.GetLeft(randomEnemy) + (randomEnemy.Width / 2) - 10;
            double bulletY = Canvas.GetTop(randomEnemy) + randomEnemy.Height;

            CreateBullet(bulletX, bulletY, 3, false);
            CheckBulletCollision(_bullets.Last(), false, _enemyBulletTimer);
        }
    }


}