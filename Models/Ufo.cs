using Avalonia.Controls;

namespace SpaceInvadersMVVM.Models;

public class Ufo
{
    public bool IsDestroyed { get; set; } = false;
    public Image? Sprite { get; set; } = new()
    {
        Width = 60,
        Height = 40,
    };

}