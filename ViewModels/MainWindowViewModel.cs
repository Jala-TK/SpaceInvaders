using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using ReactiveUI;
using SpaceInvadersMVVM.Models;

namespace SpaceInvadersMVVM.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public Player Player { get; set; }
        private double PlayerSpeed { get; set; }
        
        private ObservableCollection<Score> _scores = null!;

        public ObservableCollection<Score> Scores
        {
            get => _scores;
            set => this.RaiseAndSetIfChanged(ref _scores, value);
        }
        
        private readonly Subject<int> _playerLifeSubject = new Subject<int>();
        private IObservable<int> PlayerLifeObservable => _playerLifeSubject;
        public ReactiveCommand<Unit, Unit> MoveLeftCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> MoveRightCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ShootCommand { get; private set; }
        
        
        public MainWindowViewModel()
        {
            Player = new Player();
            PlayerSpeed = 5.0;
            Scores = LoadScoresFromCsv();
            // Configurar comandos
            MoveLeftCommand = ReactiveCommand.Create(MoveLeft);
            MoveRightCommand = ReactiveCommand.Create(MoveRight);
            ShootCommand = ReactiveCommand.Create(Shoot);

            // Lógica assíncrona (caso necessário) pode ser adicionada usando await
            MoveLeftCommand.Subscribe(_ => MoveLeft());
            MoveRightCommand.Subscribe(_ => MoveRight());
            ShootCommand.Subscribe(_ => Shoot());
            
            PlayerLifeObservable
                .Subscribe(life =>
                {
                    Console.WriteLine("Vida do jogador: " + life);
                    if (life <= 0)
                    {
                        ShowGameOverScreen();
                    }
                });
            
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
            Console.WriteLine("Mostrando tela de Game Over");
            GameOver?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler? GameOver;

    }
}