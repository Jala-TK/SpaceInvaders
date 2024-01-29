using Avalonia.Controls;

namespace SpaceInvaders.Models;

public class Shield
{
    public double Life { get; set; }
    public Image Sprite { get; set; }  // Corrigindo o nome da propriedade

    public Shield()
    {
        Life = 100;
        Sprite = new Image
        {
            Width = 80,
            Height = 60,
        };
    }
}