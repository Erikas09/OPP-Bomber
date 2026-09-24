using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class MapRenderer
{
    private const int TileSize = 40;

    public void Draw(
        Graphics graphics,
        IReadOnlyCollection<MapCellResponse> cells)
    {
        foreach (MapCellResponse cell in cells)
        {
            Rectangle rectangle = new(
                cell.X * TileSize,
                cell.Y * TileSize,
                TileSize,
                TileSize);

            Brush brush = GetBrush(cell.Type);

            graphics.FillRectangle(
                brush,
                rectangle);

            graphics.DrawRectangle(
                Pens.Gray,
                rectangle);
        }
    }

    private static Brush GetBrush(MapCellType type)
    {
        return type switch
        {
            MapCellType.Floor => Brushes.LightGray,
            MapCellType.DestructibleWall => Brushes.SaddleBrown,
            MapCellType.IndestructibleWall => Brushes.DarkGray,
            _ => Brushes.Magenta,
        };
    }
}
