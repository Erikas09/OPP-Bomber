using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class BombRenderer
{
    private const int TileSize = 40;
    private const int BombSize = 24;

    public static void Draw(
        Graphics graphics,
        BombResponse bomb)
    {
        int centerX = bomb.X * TileSize + TileSize / 2;
        int centerY = bomb.Y * TileSize + TileSize / 2;

        int x = centerX - BombSize / 2;
        int y = centerY - BombSize / 2;

        graphics.FillEllipse(
            Brushes.Black,
            x,
            y,
            BombSize,
            BombSize);
    }
}