using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Data;
using SpaceInvadersMVVM.Models;
using SpaceInvadersMVVM.ViewModels;

namespace SpaceInvadersMVVM.Views;

public partial class MainWindow : Window

{
    private Canvas? _gameCanvas;
    private Player? _player;
    private Ufo _ufo = new();
    private TextBlock _scoreTextBlock = null!;
    private TextBlock _playerLifeTextBlock = null!;
    private bool _canShoot = true;
    private bool _gameOn;
    private bool _possibleStart;
    private bool _playGame;
    private double _moveSpeed;
    private double _moveSpeedDefault = 8.0;
    private List<Invader> _enemies = [];
    private List<Barrier> _shields = [];
    private List<Bullet> _bullets = [];
    private DispatcherTimer? _timer;
    private double _invadersDirection = 1; // 1 para direita, -1 para esquerda
    private double _ufoDirection = 1; // 1 para direita, -1 para esquerda
    private SoundFx _explosionSound;
    private SoundFx _playerBulletSound;
    private SoundFx _enemyBulletSound;
    private SoundFx _backgroundSound;
    private SoundFx _moveEnemiesSound;
    private SoundFx _moveUfoSound;
    private DispatcherTimer? _enemyBulletTimer;
    public int AlienCount { get; private set; }
    private MainWindowViewModel _viewModel = null!;
    private StartScreen _startScreen = null!;
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _playerBulletSound = new SoundFx("1.wav");
        _enemyBulletSound = new SoundFx("3.wav");
        _moveEnemiesSound = new SoundFx("6.wav");
        _explosionSound = new SoundFx("explosion.wav");
        _backgroundSound = new SoundFx("backgroundmusic.mpeg");
        _moveUfoSound = new SoundFx("ufo_lowpitch.wav");
        
        NewGame();
        
        KeyDown += KeyStart;

    }

    private void NewGame()
    {
        _viewModel = new MainWindowViewModel();
        _player = new Player();
        DataContext = _viewModel;

        _startScreen = new StartScreen(_viewModel);
        Content = _startScreen;
        _possibleStart = true;
        _startScreen.StartGameClicked += StartScreen_StartGameClicked!;
    }

    private void KeyStart(object? sender, KeyEventArgs e)
    {
        if (_possibleStart)
        {
            if (e.Key == Key.Enter)
            {
                StartGame();
            }
        }

    }

    private void StartScreen_StartGameClicked(object sender, EventArgs e)
    {
        if (_possibleStart)
        {
           StartGame();
        }
    }


    private void StartGame()
    {
        ClearWindow();

        InitializeGameComponents();
        _possibleStart = false;
        _gameOn = true;
        
        Content = _gameCanvas;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _timer.Tick += (_, _) =>
        {
            if (AlienCount <= 0)
            {
                GenerateNewAliens();
            }
            _moveEnemiesSound.Play(10);
            MoveEnemies();
            _moveEnemiesSound.Play(10);
        };
        _timer.Start();

        // Ativar o OVNI a cada 2 minutos
        var activationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        activationTimer.Tick += (_, _) =>
        {
            if (_gameOn)
            {
                Dispatcher.UIThread.InvokeAsync(GenerateUfo);
            }
        };
        activationTimer.Start();
    }
    private void GenerateUfo()
    {
        var direction = new Random().Next(0, 2);
        _ufoDirection = (direction == 0 ? -2 : 2);

        var ufo = new Ufo();
        ufo.Sprite!.Source = this.FindControl<Image>("UFO")?.Source;

        // Define a posição inicial do OVNI
        if (_ufoDirection == -2)
        {
            Canvas.SetLeft(ufo.Sprite!, 1280);
        }
        else
        {
            Canvas.SetLeft(ufo.Sprite!, 0);
        }
        Canvas.SetTop(ufo.Sprite!, 20);

        // Inicia o movimento do OVNI
        _gameCanvas?.Children.Add(ufo.Sprite);
        _ufo = ufo;
        MoveUfo();
    }

    private void MoveUfo()
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(20) // Velocidade
        };
        timer.Tick += (_, _) =>
        {
            
            if (_ufo.IsDestroyed)
            {
                _gameCanvas!.Children.Remove(_ufo.Sprite!);
            }
            else
            {
                _moveUfoSound.Play(10);
            }
            // Verifica se a nave do jogador está mostrando na tela
            if (_player != null && _gameCanvas != null &&
                Canvas.GetLeft(_player.Sprite!) < _gameCanvas.Bounds.Width &&
                Canvas.GetLeft(_player.Sprite!) + _player.Sprite!.Width > 0 &&
                Canvas.GetTop(_player.Sprite) < _gameCanvas.Bounds.Height &&
                Canvas.GetTop(_player.Sprite) + _player.Sprite.Height > 0)
            {
                
                Canvas.SetLeft(_ufo.Sprite!, Canvas.GetLeft(_ufo.Sprite!) + _ufoDirection);

                // Mover o OVNI para a direção selecionada
                if (Canvas.GetLeft(_ufo.Sprite!) > _gameCanvas.Bounds.Width && Canvas.GetRight(_ufo.Sprite!) < 0) 
                { 
                    // Remover o OVNI da tela
                    timer.Stop(); // Parar o movimento do OVNI
                }
            }
        };
        timer.Start();
        
    }

    private void InitializeGameComponents()
    {
        _backgroundSound.PlayInLoop(1);

        _playGame = true;
        _gameCanvas = this.FindControl<Canvas>("GameCanvas");
        _player = _viewModel.Player;
        _player.Sprite!.Source = this.FindControl<Image>("SpaceShip")!.Source;
        _gameCanvas!.Children.Add(_player.Sprite);
        MoveSpaceShip();
        _enemyBulletTimer = new DispatcherTimer();
        _enemyBulletTimer.Interval = TimeSpan.FromMilliseconds(2000); // Intervalo do tiro dos inimigos
        _enemyBulletTimer.Tick += EnemyShoot!;
        _enemyBulletTimer.Start();

        const int numShields = 4;
        const double shieldMargin = 80;

        for (var i = 0; i < numShields; i++)
        {
            var barrier = new Barrier();
            barrier.Sprite!.Source = this.FindControl<Image>("Shield")?.Source;

            Canvas.SetLeft(barrier.Sprite, i * (1280.00 / numShields) + shieldMargin);
            Canvas.SetTop(barrier.Sprite, 600 - barrier.Sprite.Height - 100 + 70);

            _gameCanvas!.Children.Add(barrier.Sprite);
            _shields.Add(barrier);
        }

        GenerateNewAliens();
        AlienCount = _enemies.Count;

        KeyDown += OnKeyPressed;

        _scoreTextBlock = new TextBlock
        {
            FontFamily = new FontFamily("Joystix Monospace"),
            FontSize = 16,
            TextAlignment = TextAlignment.Left
        };
        Canvas.SetLeft(_scoreTextBlock, 5);
        Canvas.SetTop(_scoreTextBlock, 5);
        _scoreTextBlock.Bind(TextBlock.TextProperty, new Binding("Score") { Source = _viewModel });

        _playerLifeTextBlock = new TextBlock
        {
            FontFamily = new FontFamily("Joystix Monospace"),
            FontSize = 20,
            TextAlignment = TextAlignment.Right
        };
        Canvas.SetRight(_playerLifeTextBlock, 5);
        Canvas.SetTop(_playerLifeTextBlock, 5);
        _playerLifeTextBlock.Bind(TextBlock.TextProperty, new Binding("PlayerLife") { Source = _viewModel });

        _gameCanvas!.Children.Add(_scoreTextBlock);
        _gameCanvas!.Children.Add(_playerLifeTextBlock);


        // Inicialize o conteúdo do jogo
        _viewModel.GameOver += (_, _) =>
        {
            // Lógica para mostrar a tela de game over na janela atual
            _backgroundSound.Stop();
            StopGame();
            var gameOverContent = new StackPanel
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            var text = new TextBlock
            {
                Text = "Game Over \n" + _viewModel.Score, FontSize = 24,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center

            };
            gameOverContent.Children.Add(text);
            Content = gameOverContent;

            var nicknameTextBlock = new TextBlock()
            {
                Text = "\nSalve seu score:",
                Width = 200,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            };
            gameOverContent.Children.Add(nicknameTextBlock);

            var nicknameTextBox = new TextBox
            {
                Width = 200,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Watermark = "Enter your nickname"
            };
            gameOverContent.Children.Add(nicknameTextBox);

            var saveButton = new Button
            {
                Content = "Save Score",
                Width = 100,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            saveButton.Click += (_, _) =>
            {
                if (!string.IsNullOrEmpty(nicknameTextBox.Text))
                {
                    _viewModel.SaveScoreToCsv(nicknameTextBox.Text);
                    NewGame();
                }
            };

            gameOverContent.Children.Add(saveButton);
            
            var menuButton = new Button
            {
                Content = "Voltar para o menu",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            menuButton.Click += (_, _) =>
            {
                NewGame();
            };

            gameOverContent.Children.Add(menuButton);
        };
    }
    
    
    private void GenerateNewAliens()
    {
        _enemies = new List<Invader>();

        _moveSpeedDefault++;
        _moveSpeed = _moveSpeedDefault;
        const int rows = 5;
        const int cols = 11;
        const double enemyMarginWidth = 25;
        const double enemyMarginHeight = 10;

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

                Canvas.SetLeft(enemy.Sprite, col * (enemy.Sprite.Width + enemyMarginWidth));
                Canvas.SetTop(enemy.Sprite, row * (enemy.Sprite.Height + enemyMarginHeight) + 90);

                _gameCanvas?.Children.Add(enemy.Sprite);
                _enemies.Add(enemy);
            }
        }
        AlienCount = _enemies.Count;

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnKeyPressed(object? sender, KeyEventArgs e)
    {

        switch (e.Key)
        {
            case Key.Left or Key.A:
                _viewModel.MoveLeftCommand.Execute().Subscribe();
                MoveSpaceShip();
                break;
            case Key.Right or Key.D:
                _viewModel.MoveRightCommand.Execute().Subscribe();
                MoveSpaceShip();
                break;
            case Key.Space:
                if (_canShoot && _playGame)
                {
                    var bulletX = _player!.X + (_player.Sprite!.Width / 2) - 10;
                    var bulletY = _player.Y;
                    CreateBullet(bulletX, bulletY, 20, true);
                    _canShoot = false; // Impede que o jogador atire novamente
                }
                break;
        }



    }

    private void MoveSpaceShip()
    {
        if (!_player!.IsDestroyed)
        {
            Canvas.SetLeft(_player!.Sprite!, _player.X);
            Canvas.SetTop(_player.Sprite!, _player.Y);
        }
    }


    private void MoveEnemies()
    {
        var shouldMoveDown = false;

        foreach (var enemy in _enemies)
        {
            if (enemy.IsDestroyed)
            {
                _gameCanvas!.Children.Remove(enemy.Sprite!);
                continue;
            }
            var x = Canvas.GetLeft(enemy.Sprite!);
            var moveSpeed = _moveSpeed * _invadersDirection;

            if (x + moveSpeed < 0 || x + moveSpeed > _gameCanvas!.Bounds.Width - enemy.Sprite!.Width)
            {
                _invadersDirection *= -1;
                _moveSpeed += 0.5;
                shouldMoveDown = true;
                break;
            }
        }

        foreach (var enemy in _enemies)
        {
            if (enemy.IsDestroyed)
            {
                continue;
            }
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

        var bullet = new Bullet(x, y, speed, isPlayerBullet);
        bullet.Sprite!.Source = this.FindControl<Image>("Bala")?.Source;


        if (!isPlayerBullet)
        {
            _playerBulletSound.Play(10);

            bullet.Sprite.RenderTransform = new RotateTransform(180);
        }
        else
        {
            _enemyBulletSound.Play(10);

        }

        Canvas.SetLeft(bullet.Sprite, x);
        Canvas.SetTop(bullet.Sprite, y);

        _gameCanvas!.Children.Add(bullet.Sprite);
        _bullets.Add(bullet);

        // Iniciar o movimento da bala
        var bulletTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(20)
        };
        bulletTimer.Tick += (_, _) =>
        {
            var bulletY = Canvas.GetTop(bullet.Sprite);

            if (isPlayerBullet)
            {
                bulletY -= speed;
            }
            else
            {
                bulletY += speed;
            }
            Canvas.SetTop(bullet.Sprite, bulletY);


            // Verifica a colisão com inimigos ou jogador
            CheckBulletCollision(bullet, isPlayerBullet, bulletTimer);

            // Remove a bala se estiver fora da tela
            if (bulletY < 0 || bulletY > _gameCanvas.Bounds.Height)
            {
                _gameCanvas.Children.Remove(bullet.Sprite);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Para o timer da bala
                if (isPlayerBullet)
                {
                    _canShoot = true;
                }
            }
        };

        bulletTimer.Start();
    }

    private void CheckBulletCollision(Bullet bullet, bool isPlayerBullet, DispatcherTimer bulletTimer)
    {
        // Verifica colisão com o OVNI
        if (isPlayerBullet)
        {
            // Verifica colisão com inimigos
            foreach (var enemy in _enemies)
            {
                if (enemy.IsDestroyed == false)
                {
                    if (CheckCollision(bullet.Sprite!, enemy.Sprite!))
                    {
                        _explosionSound.Play(10);
                        _viewModel.UpdateScore(enemy.Score);
                        // _viewModel.UpdateScore(400);

                        enemy.IsDestroyed = true;
                        enemy.Sprite!.Source = this.FindControl<Image>("Destruction")!.Source;

                        // Iniciar o timer para remover o inimigo após 1 segundo
                        var removeEnemyTimer = new DispatcherTimer
                        {
                            Interval = TimeSpan.FromMilliseconds(100)
                        };
                        removeEnemyTimer.Tick += (_, _) =>
                        {
                            _gameCanvas!.Children.Remove(enemy.Sprite!);
                            _enemies.Remove(enemy);
                            AlienCount --;
                            removeEnemyTimer.Stop();
                        };
                        removeEnemyTimer.Start();

                        _gameCanvas!.Children.Remove(bullet.Sprite!);
                        _bullets.Remove(bullet);
                        bulletTimer.Stop(); // Para o timer da bala
                        _canShoot = true;
                        break;
                    }

                }
            }

            //Colisão com o Ufo (OVNI)
            if (_ufo.Sprite != new Ufo().Sprite && _ufo.IsDestroyed == false)
            {
                if (CheckCollision(bullet.Sprite!, _ufo.Sprite!))
                {
                    _explosionSound.Play(10);
                    var randomScore = new Random().Next(10, 51) * 10;
                    _viewModel.UpdateScore(randomScore);

                    _ufo.IsDestroyed = true;
                    _ufo.Sprite!.Source = this.FindControl<Image>("UFODestroyer")!.Source;

                    // Inicia o timer para remover o inimigo após 1 segundo
                    var removeUfoTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(1)
                    };
                    removeUfoTimer.Tick += (_, _) =>
                    {
                        _gameCanvas!.Children.Remove(_ufo.Sprite!);
                        removeUfoTimer.Stop();
                    };
                    removeUfoTimer.Start();

                    _gameCanvas!.Children.Remove(bullet.Sprite!);
                    _bullets.Remove(bullet);
                    bulletTimer.Stop(); // Para o timer da bala
                    _canShoot = true;
                }
            }

        }
        else
        {
            // Verifica colisão com jogador
            if (CheckCollision(bullet.Sprite!, _player!.Sprite!))
            {
                _explosionSound.Play(10);
                _viewModel.LifeUpdate(-1);

                // Substitui o sprite atual pelo sprite de destruição
                _gameCanvas!.Children.Remove(_player.Sprite!);
                _player.Sprite!.Source = this.FindControl<Image>("SpaceShipDestroyer")!.Source;
                _gameCanvas!.Children.Add(_player.Sprite);
                _player.IsDestroyed = true;

                // Inicia o timer para remover o jogador após 1 segundo
                var removePlayerTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(200)
                };
                removePlayerTimer.Tick += (_, _) =>
                {
                    _gameCanvas!.Children.Remove(_player.Sprite!);
                    _player.IsDestroyed = false;
                    _player.Sprite.Source = this.FindControl<Image>("SpaceShip")!.Source;
                    _gameCanvas!.Children.Add(_player.Sprite);
                    removePlayerTimer.Stop();
                };
                removePlayerTimer.Start();

                _gameCanvas!.Children.Remove(bullet.Sprite!);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Para o timer da bala
                _canShoot = true;
            }
        }

        //Colisão dos inimigos com o player
        foreach (var enemy in _enemies)
        {
            if (enemy.IsDestroyed == false)
            {
                if (CheckCollision(enemy.Sprite!, _player!.Sprite!))
                {
                    _explosionSound.Play(10);
                    _viewModel.LifeUpdate(-(_player.Life));
                    break;
                }
            }
        }

        // Verifica colisão com barreiras
        foreach (var shield in _shields)
        {
            if (CheckCollision(bullet.Sprite!, shield.Sprite!))
            {
                // Remove a bala e danificar a barreira
                _gameCanvas!.Children.Remove(bullet.Sprite!);
                _bullets.Remove(bullet);
                bulletTimer.Stop(); // Para o timer da bala
                if (isPlayerBullet)
                {
                    _canShoot = true;
                }
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
                        return; // Sai do switch quando o escudo for removido
                }

                shield.Life -= 20;

                break;
            }
        }

        // Remove a bala se estiver fora da tela
        if (Canvas.GetTop(bullet.Sprite!) < 0 || Canvas.GetTop(bullet.Sprite!) > _gameCanvas!.Bounds.Height)
        {
            _gameCanvas!.Children.Remove(bullet.Sprite!);
            _bullets.Remove(bullet);
            bulletTimer.Stop(); // Para o timer da bala
            if (isPlayerBullet)
            {
                _canShoot = true;
            }
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
        if (_playGame)
        {
            var random = new Random();

            // Escolhe um inimigo aleatório da primeira fileira de cima para atirar
            var enemiesCanShoot = _enemies.Where(enemy => enemy.Row == 0).ToList();

            if (enemiesCanShoot.Count > 0)
            {
                var randomEnemyIndex = random.Next(enemiesCanShoot.Count);
                var randomEnemy = enemiesCanShoot[randomEnemyIndex].Sprite;

                var bulletX = Canvas.GetLeft(randomEnemy!) + (randomEnemy!.Width / 2) - 10;
                var bulletY = Canvas.GetTop(randomEnemy) + randomEnemy.Height;

                CreateBullet(bulletX, bulletY, 3, false);
                CheckBulletCollision(_bullets.Last(), false, _enemyBulletTimer!);
            }
        }


    }

    private void StopGame()
    {
        _playGame = false;
        _gameOn = false;
        _backgroundSound.Stop();
        _playerBulletSound.Stop();
        _enemyBulletSound.Stop();
        _explosionSound.Stop();
        _moveEnemiesSound.Stop();
        _timer?.Stop();
        _enemyBulletTimer?.Stop();
        KeyDown -= OnKeyPressed;

        // Remove todos os inimigos da tela
        foreach (var enemy in _enemies)
        {
            _gameCanvas?.Children.Remove(enemy.Sprite!);
        }
        _enemies.Clear();

        // Remove todos os tiros da tela
        foreach (var bullet in _bullets)
        {
            _gameCanvas?.Children.Remove(bullet.Sprite!);
        }
        _bullets.Clear();

        // Remove o jogador da tela
        if (_player != null)
        {
            _gameCanvas?.Children.Remove(_player.Sprite!);
        }

        // Remove todos os escudos da tela
        foreach (var shield in _shields)
        {
            _gameCanvas?.Children.Remove(shield.Sprite!);
        }
        _shields.Clear();

        // Remove o OVNI da tela
        if (_ufo.Sprite != null)
        {
            _gameCanvas?.Children.Remove(_ufo.Sprite!);
        }

        // Limpa o conteúdo do canvas do jogo
        _gameCanvas?.Children.Clear();

        // Define o conteúdo da janela como nulo
        Content = null;

    }

    private void ClearWindow()
    {
        StopGame();
        Content = new StackPanel();
    }


}