using Avalonia.Controls;

namespace SpaceInvadersMVVM.Models;

public class Ufo
{
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public bool IsDestroyed { get; set; } = false;
    public Image? Sprite { get; set; } = new()
    {
        Width = 60,
        Height = 40,
    };

}