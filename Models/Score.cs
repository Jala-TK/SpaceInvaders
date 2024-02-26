using System;

namespace SpaceInvadersMVVM.Models
{
    public class Score
    {
        public Score(string nickname, int score, DateTime date)
        {
            Nickname = nickname;
            ScoreValue = score;
            Date = date;
            GetDate = Date.ToString("dd/MM/yyyy");
        }

        public Score()
        {
            throw new NotImplementedException();
        }

        public string GetDate { get; set; }
        public string Nickname { get; set; }
        public int ScoreValue { get; set; } // Renomeado para ScoreValue
        public DateTime Date { get; set; }
    }
}