using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ReactiveUI;
using SpaceInvadersMVVM.Models;

namespace SpaceInvadersMVVM.ViewModels
{
    partial class MainWindowViewModel : ViewModelBase
    {
        private int _score;
        private int _lifeScore;
        public string Score => "Score: " + _score;
        public string PlayerLife => string.Join("", Enumerable.Repeat("❤️ ", Player.Life));

        public void UpdateScore(int points)
        {
            _score += points;
            _lifeScore += points;
            this.RaisePropertyChanged(nameof(Score));

            // Verifica se a pontuação atingiu ou ultrapassou um múltiplo de 1000
            if (_lifeScore >= 1000 && Player.Life is < 6 and > 0)
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

        public void SaveScoreToCsv(string nickname)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string assetsPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "..", "Assets"));
            string filePath = Path.Combine(assetsPath, "highscore.csv");
            bool found = false;

            // Cria o arquivo CSV se não existir e escreve o cabeçalho
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Nickname,Score,Date");
                }
            }

            // Lê o arquivo CSV e verifica se o nickname já existe
            var lines = File.ReadAllLines(filePath).ToList();
            for (int i = 1; i < lines.Count; i++) // Começa de 1 para pular o cabeçalho
            {
                var data = lines[i].Split(',');
                if (data[0] == nickname)
                {
                    // Se o score for maior, atualiza o registro
                    if (_score > int.Parse(data[1]))
                    {
                        lines[i] = $"{nickname},{_score},{date}";
                    }
                    found = true;
                    break;
                }
            }

            // Se o nickname não foi encontrado, adiciona um novo registro
            if (!found && !string.IsNullOrEmpty(nickname))
            {
                lines.Add($"{nickname},{_score},{date}");
            }

            // Escreve os dados atualizados no arquivo CSV
            File.WriteAllLines(filePath, lines);
        }

        public ObservableCollection<Score> LoadScoresFromCsv()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string assetsPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "..", "Assets"));
            string filePath = Path.Combine(assetsPath, "highscore.csv");

            // Verifica se o arquivo CSV existe
            if (!File.Exists(filePath))
            {
                return new ObservableCollection<Score>(); // Retorna uma lista vazia se o arquivo não existir
            }

            // Lê o arquivo CSV e cria uma lista de objetos Score
            var scores = new ObservableCollection<Score>();
            var lines = File.ReadAllLines(filePath).Skip(1); // Pula o cabeçalho
            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (data.Length >= 3)
                {
                    scores.Add(new Score(data[0], int.Parse(data[1]), DateTime.Parse(data[2])));
                }
            }

            return scores;
        }
    }
}