using System;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using SpaceInvadersMVVM.Models;
using SpaceInvadersMVVM.Views;

namespace SpaceInvadersMVVM.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public Player Player
        {
            get; set;
        } = new();
        private double PlayerSpeed { get; set; } = 5.0;
        
        private readonly Subject<int> _playerLifeSubject = new Subject<int>();
        public IObservable<int> PlayerLifeObservable => _playerLifeSubject;


        public ReactiveCommand<Unit, Unit> MoveLeftCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> MoveRightCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ShootCommand { get; private set; }
        
        
        public MainWindowViewModel()
        {
            // Configurar comandos
            MoveLeftCommand = ReactiveCommand.Create(MoveLeft);
            MoveRightCommand = ReactiveCommand.Create(MoveRight);
            ShootCommand = ReactiveCommand.Create(Shoot);

            // Lógica assíncrona (caso necessário) pode ser adicionada usando await
            MoveLeftCommand.Subscribe(_ => MoveLeft());
            MoveRightCommand.Subscribe(_ => MoveRight());
            ShootCommand.Subscribe(_ => Shoot());
            
        }

        private void MoveLeft()
        {
            if (Player.X - PlayerSpeed >= 0)
            {
                Player.X -= PlayerSpeed;
            }
        }

        private void MoveRight()
        {
            if (Player.X + PlayerSpeed <= 800 - Player.Sprite!.Width) 
            {
                Player.X += PlayerSpeed;
            }
        }

        private void Shoot()
        {
            
        }
        
        private void ShowGameOverScreen()
        {
            // Cria e mostra a tela de game over
            Console.WriteLine("Criando a janela de Game Over");
            var gameOverWindow = new GameOverWindow();
            gameOverWindow.Show();
            // Fecha a janela atual
            (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow!.Close();
        }



    }
}