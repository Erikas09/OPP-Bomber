using Bomber.Client.Interfaces;
using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class MapRenderer : Renderer
{
    private const int TileSize = 40;

    public MapRenderer(IDrawingContext drawingContext)
        : base(drawingContext)
    {
    }

    public void Draw(
        IReadOnlyCollection<MapCellResponse> cells)
    {
        foreach (MapCellResponse cell in cells)
        {
            RectangleF rectangle = new(
                cell.X * TileSize,
                cell.Y * TileSize,
                TileSize,
                TileSize);

            Color color = GetColor(cell.Type);

            DrawingContext.FillRectangle(color, rectangle);

            DrawingContext.DrawRectangle(Color.Gray, rectangle);
        }
    }

    private static Color GetColor(MapCellType type)
    {
        return type switch
        {
            MapCellType.Floor => Color.LightGray,
            MapCellType.DestructibleWall => Color.SaddleBrown,
            MapCellType.IndestructibleWall => Color.DarkGray,
            MapCellType.IceFloor => Color.LightSkyBlue,
            MapCellType.IceDestructibleWall => Color.SlateBlue,
            MapCellType.IceIndestructibleWall => Color.DodgerBlue,
            _ => Color.Magenta,
        };
    }
}
