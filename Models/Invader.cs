using Avalonia.Controls;

namespace SpaceInvadersMVVM.Models
{
    public class Invader
    {
        public int Col { get; set; }
        public int Row { get; set; }
        public Image? Sprite { get; set; }
        public int Score { get; set; }

        public Invader(int col, int row)
        {
            Col = col;
            Row = row;
            Sprite = new Image
            {
                Width = 40,
                Height = 40
            };
            Score = CalculateScore();
        }

        private int CalculateScore()
        {
            switch (Row)
            {
                case 0:
                    return 40;
                case 1:
                case 2:
                    return 20;
                default:
                    return 10;
            }
        }
    }
}