using Bomber.Client.Interfaces;
using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class BombRenderer : Renderer
{
    private const int TileSize = 40;
    private const int BombSize = 24;

    public BombRenderer(IDrawingContext drawingContext)
        : base(drawingContext)
    {
    }

    public void Draw(BombResponse bomb)
    {
        int centerX = bomb.X * TileSize + TileSize / 2;
        int centerY = bomb.Y * TileSize + TileSize / 2;

        int x = centerX - BombSize / 2;
        int y = centerY - BombSize / 2;

        RectangleF rectangle = new(
            x,
            y,
            BombSize,
            BombSize);

        DrawingContext.FillEllipse(
            Color.Black,
            rectangle);
    }
}