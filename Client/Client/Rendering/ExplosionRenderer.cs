using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class ExplosionRenderer
{
    private const int TileSize = 40;

    public static void Draw(
        Graphics graphics,
        IReadOnlyCollection<ExplosionCell> cells)
    {
        using SolidBrush brush = new(Color.FromArgb(160, 255, 140, 0));

        foreach (ExplosionCell cell in cells)
        {
            Rectangle rectangle = new(
                cell.X * TileSize,
                cell.Y * TileSize,
                TileSize,
                TileSize);

            graphics.FillRectangle(
                brush,
                rectangle);
        }
    }
}