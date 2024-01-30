using Avalonia.Controls;

namespace SpaceInvadersMVVM.Models;

public class Player
{
    public double X { get; set; } = 350;
    public double Y { get; set; } = 500;
    public Image? Sprite { get; set; } = new()
    {
        Width = 100,
        Height = 100,
    };
}