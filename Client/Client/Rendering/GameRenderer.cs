using Bomber.Client.Models;
using System.Diagnostics;

namespace Bomber.Client.Rendering;

public sealed class GameRenderer
{
    private readonly MapRenderer _mapRenderer;
    private readonly PlayerRenderer _playerRenderer;

    private readonly BombRenderer _bombRenderer;
    public GameRenderer()
    {
        _mapRenderer = new MapRenderer();
        _playerRenderer = new PlayerRenderer();
        _bombRenderer = new BombRenderer();
    }

    public void Draw(
        Graphics graphics,
        ClientGameState gameState,
        Guid? localPlayerId)
    {
        _mapRenderer.Draw(
            graphics,
            gameState.Cells);

        List<ExplosionCell> explosions = gameState.GetActiveExplosionCells();

        ExplosionRenderer.Draw(graphics, explosions);

        foreach (BombResponse bomb in gameState.Bombs)
        {
            BombRenderer.Draw(
                graphics,
                bomb);
        }

        foreach (PlayerStateResponse player in gameState.Players)
        {
            PlayerRenderer.Draw(
                graphics,
                player,
                localPlayerId);
        }
    }
}
