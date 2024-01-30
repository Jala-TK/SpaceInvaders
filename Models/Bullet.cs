using Avalonia.Controls;

namespace SpaceInvadersMVVM.Models;

public class Bullet(double x, double y, double speed, bool isPlayer)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Speed { get; set; } = speed;
    public bool IsPlayerBullet { get; set; } = isPlayer;
    public Image? Sprite { get; set; } = new()
    {
        Width = 20,
        Height = 35,
    };


}