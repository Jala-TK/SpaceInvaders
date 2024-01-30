using Avalonia.Controls;

namespace SpaceInvadersMVVM.Models;

public class Barrier
{
    public double Life { get; set; } = 100;
    public Image? Sprite { get; set; } = new()
    {
        Width = 80,
        Height = 60,
    };
}