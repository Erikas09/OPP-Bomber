using Bomber.Client.Interfaces;
using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class PlayerRenderer : Renderer
{
    private const int PlayerSize = 30;

    public PlayerRenderer(IDrawingContext drawingContext)
        : base(drawingContext)
    {
    }

    public void Draw(
        PlayerStateResponse player,
        Guid? localPlayerId)
    {
        Color color =
            player.Id == localPlayerId
                ? Color.Blue
                : Color.Red;

        RectangleF rectangle = new(
            player.X,
            player.Y,
            PlayerSize,
            PlayerSize);

        DrawingContext.FillRectangle(
            color,
            rectangle);

        DrawingContext.DrawText(
            $"♥ {player.Lives}",
            Color.White,
            player.X,
            player.Y - 16);
    }
}
