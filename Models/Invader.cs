namespace SpaceInvaders.Models;

public class Invader
{
    public double X { get; set; }
    public double Y { get; set; }
    public int Col { get; set; }
    public int Row { get; set; }
    public Invader(int col, int row, double x, double y)
    {
        X = x;
        Y = y;
        Col = col;
        Row = row;
    }
}