using System;
using Avalonia.Threading;
using SpaceInvaders.Models;

namespace SpaceInvaders.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Player Player { get; } = new();
        public InvaderFormation InvaderFormation { get; } = new();

        
        private double _invadersX;
        private double _invadersY;
        private double _invadersSpeed = 2;
        private double _invadersDirection = 1; // 1 para direita, -1 para esquerda


        public double InvadersX
        {
            get => _invadersX;
            set
            {
                if (_invadersX != value)
                {
                    _invadersX = value;
                    OnPropertyChanged(nameof(InvadersX));
                }
            }
        }

        public double InvadersY
        {
            get => _invadersY;
            set
            {
                if (_invadersY != value)
                {
                    _invadersY = value;
                    OnPropertyChanged(nameof(InvadersY));
                }
            }
        }
        
        public MainWindowViewModel()
        {
            InitializeInvadersMovement();
        }

        private void InitializeInvadersMovement()
        {
            // Use um despachante para atualizar a interface do usuário em um intervalo
            var dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(110) };
            dispatcherTimer.Tick += (sender, args) => MoveInvaders();
            dispatcherTimer.Start();
        }

        private void MoveInvaders()
        {
            // Atualiza a posição dos invasores
            InvadersX += _invadersSpeed * _invadersDirection;

            // Verifica as bordas e inverte a direção se atingir uma borda
            if (InvadersX < 0 || InvadersX > 400) // Ajuste conforme necessário para o tamanho do WrapPanel
            {
                _invadersDirection *= -1;
                InvadersY += 20; // Move para baixo
            }
        }
    }
}