using Bomber.Client.Interfaces;
using Bomber.Client.Models;
using Bomber.Client.Rendering.Animations;

namespace Bomber.Client.Rendering;

public sealed class GameRenderer
{
    private readonly IDrawingContext _drawingContext;

    private readonly MapRenderer _mapRenderer;
    private readonly PlayerRenderer _playerRenderer;
    private readonly BombRenderer _bombRenderer;
    private readonly PowerUpRenderer _powerUpRenderer;
    private readonly ExplosionRenderer _explosionRenderer;

    AnimationManager _animationManager;

    public GameRenderer(IDrawingContext drawingContext)
    {
        _animationManager = new AnimationManager();
        _drawingContext = drawingContext;

        _mapRenderer = new MapRenderer(drawingContext);
        _playerRenderer = new PlayerRenderer(_animationManager, drawingContext);
        _bombRenderer = new BombRenderer(drawingContext);
        _powerUpRenderer = new PowerUpRenderer(drawingContext);
        _explosionRenderer = new ExplosionRenderer(drawingContext);
    }

    public void Draw(
        Graphics graphics,
        ClientGameState gameState,
        Guid? localPlayerId)
    {

        _animationManager.UpdateData(gameState.Players);

        _drawingContext.BeginFrame(graphics);

        _mapRenderer.Draw(gameState.Cells);

        _powerUpRenderer.Draw(gameState.PowerUps);

        List<GridPosition> explosionCells = [];

        foreach (ExplosionResponse explosion in gameState.Explosions)
            explosionCells.AddRange(explosion.Cells);

        _explosionRenderer.Draw(explosionCells);

        foreach (BombResponse bomb in gameState.Bombs)
            _bombRenderer.Draw(bomb);

        foreach (PlayerStateResponse player in gameState.Players)
            _playerRenderer.Draw(player, localPlayerId);
    }
}