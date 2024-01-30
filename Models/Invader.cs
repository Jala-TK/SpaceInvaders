using Avalonia.Controls;

namespace SpaceInvadersMVVM.Models
{
    public class Invader(int col, int row)
    {
        public int Col { get; set; } = col;
        public int Row { get; set; } = row;
        public Image? Sprite { get; set; } = new()
        {
            Width = 40,
            Height = 40,
        };
    }
}
