using Bomber.Client.Models;

namespace Bomber.Client.Rendering;

public sealed class GameRenderer
{
    private readonly MapRenderer _mapRenderer;
    private readonly PlayerRenderer _playerRenderer;

    public GameRenderer()
    {
        _mapRenderer = new MapRenderer();
        _playerRenderer = new PlayerRenderer();
    }

    public void Draw(
        Graphics graphics,
        ClientGameState gameState,
        Guid? localPlayerId)
    {
        _mapRenderer.Draw(
            graphics,
            gameState.Cells);

        foreach (PlayerStateResponse player in gameState.Players)
        {
            PlayerRenderer.Draw(
                graphics,
                player,
                localPlayerId);
        }
    }
}
