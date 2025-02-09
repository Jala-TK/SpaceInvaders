﻿using Avalonia.Controls;

namespace SpaceInvadersMVVM.Models;

public class Player
{
    public double X { get; set; } = 350;
    public double Y { get; set; } = 600;
    public bool IsDestroyed { get; set; }
    public Image? Sprite { get; set; } = new()
    {
        Width = 100,
        Height = 100,
    };

    public int Life { get; set; } = 3;
}