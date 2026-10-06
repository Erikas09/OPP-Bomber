using Bomber.Client.Interfaces;
using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class PowerUpRenderer : Renderer
{
    private const int TileSize = 40;

    public PowerUpRenderer(IDrawingContext drawingContext)
        : base(drawingContext)
    {
    }

    public void Draw(IReadOnlyCollection<PowerUpResponse> powerUps)
    {
        foreach (PowerUpResponse powerUp in powerUps)
        {
            int size = TileSize / 2;

            RectangleF rectangle = new(
                powerUp.X * TileSize + (TileSize - size) / 2,
                powerUp.Y * TileSize + (TileSize - size) / 2,
                size,
                size);

            Color color = GetColor(powerUp.Type);

            DrawingContext.FillRectangle(color, rectangle);

            DrawingContext.DrawRectangle(Color.Gray, rectangle);
        }
    }

    private static Color GetColor(PowerUpType type)
    {
        return type switch
        {
            PowerUpType.Speed => Color.Aqua,
            PowerUpType.BlastRadius => Color.Chartreuse,
            PowerUpType.ExtraBomb => Color.Red,
            _ => Color.Magenta,
        };
    }

}