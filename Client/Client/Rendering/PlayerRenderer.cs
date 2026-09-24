using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class PlayerRenderer
{
    private const int PlayerSize = 30;

    public static void Draw(
        Graphics graphics,
        PlayerStateResponse player,
        Guid? localPlayerId)
    {
        Brush brush =
            player.Id == localPlayerId
                ? Brushes.Blue
                : Brushes.Red;

        graphics.FillRectangle(
            brush,
            player.X,
            player.Y,
            PlayerSize,
            PlayerSize);

        graphics.DrawString(
        $"♥ {player.Lives}",
        SystemFonts.DefaultFont,
        Brushes.White,
        player.X,
        player.Y - 16);
    }
}
