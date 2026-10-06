using Bomber.Client.Interfaces;
using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class ExplosionRenderer : Renderer
{
    private const int TileSize = 40;

    public ExplosionRenderer(IDrawingContext drawingContext)
        : base(drawingContext)
    {
    }

    public void Draw(List<GridPosition> cells)
    {
        Color color =
            Color.FromArgb(
                160,
                255,
                140,
                0);

        foreach (GridPosition cell in cells)
        {
            RectangleF rectangle = new(
                cell.X * TileSize,
                cell.Y * TileSize,
                TileSize,
                TileSize);

            DrawingContext.FillRectangle(color, rectangle);
        }
    }
}