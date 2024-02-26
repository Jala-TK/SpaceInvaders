using Avalonia.Controls;

namespace SpaceInvadersMVVM.Models;

public class Ufo
{
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public Image? Sprite { get; set; } = new()
    {
        Width = 1000,
        Height = 60,
    };

}