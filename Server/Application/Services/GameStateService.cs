using Bomber.Domain.Models;

namespace Bomber.Application.Services;

public sealed class GameStateService
{
    private const float MovementSpeed = 5f;
    private const float PlayerSize = 30f;

    private const float MapWidth = 600f;
    private const float MapHeight = 400f;

    private readonly Lock _lock = new();

    private readonly Dictionary<string, PlayerState> _players = [];

    public void AddPlayer(string id)
    {
        lock (_lock)
        {
            _players[id] = new PlayerState
            {
                Id = id,
                X = 50,
                Y = 50
            };
        }
    }

    public void RemovePlayer(string id)
    {
        lock (_lock)
        {
            _players.Remove(id);
        }
    }

    public void MovePlayer(
        string id,
        bool up,
        bool down,
        bool left,
        bool right)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(id, out var player))
                return;

            if (up)
                player.Y -= MovementSpeed;

            if (down)
                player.Y += MovementSpeed;

            if (left)
                player.X -= MovementSpeed;

            if (right)
                player.X += MovementSpeed;

            player.X = Math.Clamp(
                player.X,
                0,
                MapWidth - PlayerSize);

            player.Y = Math.Clamp(
                player.Y,
                0,
                MapHeight - PlayerSize);
        }
    }

    public List<PlayerState> GetPlayers()
    {
        lock (_lock)
        {
            return [.. _players.Values
                .Select(x => new PlayerState
                {
                    Id = x.Id,
                    X = x.X,
                    Y = x.Y
                })];
        }
    }
}
