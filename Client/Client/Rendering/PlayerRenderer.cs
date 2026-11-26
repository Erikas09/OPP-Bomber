using Bomber.Client.Interfaces;
using Bomber.Client.Models;
using Bomber.Client.Rendering.Animations;

namespace Bomber.Client.Rendering;

public sealed class PlayerRenderer : Renderer, IDisposable
{
    private const int PlayerSize = 32;

    readonly Dictionary<string, SpriteSheet> _spriteSheets = new();

    AnimationManager _animationManager;

    public PlayerRenderer(AnimationManager manager, IDrawingContext drawingContext) : base(drawingContext)
    {
        _animationManager = manager;

        _spriteSheets.Add("idle", new SpriteSheet("Assets/player_idle.png", 32, 32, 0, -3));
        _spriteSheets.Add("move", new SpriteSheet("Assets/player_move.png", 32, 32, 0, -3));
        _spriteSheets.Add("dead", new SpriteSheet("Assets/player_dead.png", 32, 32, 0, -3));
        _spriteSheets.Add("victory", new SpriteSheet("Assets/player_victory.png", 32, 32, 0, -3));
    }

    public void Draw(PlayerStateResponse player, Guid? localPlayerId)
    {
        RectangleF rectangle = new(player.X, player.Y, PlayerSize, PlayerSize);

        string sheetName = _animationManager.GetSheetName(player.Id);
        int column = _animationManager.GetSpriteSheetColumn(player.Id);
        int row = _animationManager.GetSpriteSheetRow(player.Id);

        _spriteSheets[sheetName].Draw(DrawingContext, column, row, rectangle);
        DrawingContext.DrawText($"♥ {player.Lives}", Color.White, player.X, player.Y - 16);
    }

    public void Dispose()
    {
        foreach (var sheet in _spriteSheets)
            sheet.Value.Dispose();
    }
}