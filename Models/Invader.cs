using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace SpaceInvaders.Models
{
    public class Invader
    {
        public int Col { get; set; }
        public int Row { get; set; }
        public Image Sprite { get; set; }  // Corrigindo o nome da propriedade

        public Invader(int col, int row)
        {
            Col = col;
            Row = row;

            Sprite = new Image
            {
                Width = 40,
                Height = 40,
            };
        }
        
    }
}
