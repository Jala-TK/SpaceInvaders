using System;
using ReactiveUI;

namespace SpaceInvadersMVVM.ViewModels
{
    partial class MainWindowViewModel : ViewModelBase
    {
        private int _score;
        private int _lifeScore;

        public string Score => "Score: " + _score;
        public string PlayerLife => "Lives: " + Player.Life;

        public void UpdateScore(int points)
        {
            _score += points;
            _lifeScore += points;
            this.RaisePropertyChanged(nameof(Score));

            // Verifica se a pontuação atingiu ou ultrapassou um múltiplo de 1000
            if (_lifeScore >= 1000 && Player.Life is < 6 and > 0 )
            {
                int remainingScore = _lifeScore - 1000;
                LifeUpdate(1);
                _lifeScore = remainingScore;
            }
        }
        
        public void LifeUpdate(int newLife)
        {
            Player.Life += newLife;
            _playerLifeSubject.OnNext(Player.Life);
            this.RaisePropertyChanged(nameof(PlayerLife));
        }
    }
}
