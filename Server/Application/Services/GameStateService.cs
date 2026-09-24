using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;

namespace Bomber.Application.Services;

public sealed class GameStateService
{
    private const float MovementStep = 5f;
    private const float PlayerSize = 30f;
    private const int TileSize = 40;

    private readonly Lock _lock = new();

    private readonly Dictionary<Guid, Player> _players = [];

    private readonly Map _map;

    public GameStateService()
    {
        _map = CreateTestMap();
    }

    public void AddPlayer(
        Guid playerId,
        string playerName)
    {
        lock (_lock)
        {
            if (_players.ContainsKey(playerId))
                return;

            if (_players.Count >= _map.SpawnPoints.Count)
                throw new InvalidOperationException("No spawn point is available.");

            SpawnPoint spawnPoint =
                _map.SpawnPoints.ElementAt(_players.Count);

            Position position = new(
                spawnPoint.Position.X * TileSize,
                spawnPoint.Position.Y * TileSize);

            Player player = new(
                playerId,
                playerName,
                position,
                lives: 3,
                bombLimit: 1,
                explosionRadius: 1,
                speed: 120f);

            _players.Add(
                playerId,
                player);
        }
    }

    public void RemovePlayer(Guid playerId)
    {
        lock (_lock)
        {
            _players.Remove(playerId);
        }
    }

    public void MovePlayer(
        Guid playerId,
        bool up,
        bool down,
        bool left,
        bool right)
    {
        lock (_lock)
        {
            bool found = _players.TryGetValue(playerId, out Player? player);

            if (!found || player is null)
                return;

            if (player.State == PlayerState.Dead)
                return;

            float movementX = 0f;
            float movementY = 0f;

            if (up)
                movementY -= MovementStep;

            if (down)
                movementY += MovementStep;

            if (left)
                movementX -= MovementStep;

            if (right)
                movementX += MovementStep;

            Position horizontalPosition = new(
                player.Position.X + movementX,
                player.Position.Y);

            if (CanPlayerMoveTo(horizontalPosition))
                player.MoveTo(horizontalPosition);

            Position verticalPosition = new(
                player.Position.X,
                player.Position.Y + movementY);

            if (CanPlayerMoveTo(verticalPosition))
                player.MoveTo(verticalPosition);
        }
    }

    public List<Player> GetPlayers()
    {
        lock (_lock)
            return [.. _players.Values];
    }

    public Map GetMap()
    {
        return _map;
    }

    private bool CanPlayerMoveTo(Position position)
    {
        int left = (int)(position.X / TileSize);

        int right = (int)((position.X + PlayerSize - 1) / TileSize);

        int top = (int)(position.Y / TileSize);

        int bottom = (int)((position.Y + PlayerSize - 1) / TileSize);

        GridPosition topLeft = new(left, top);

        GridPosition topRight = new(right, top);

        GridPosition bottomLeft = new(left, bottom);

        GridPosition bottomRight = new(right, bottom);

        return _map.IsWalkable(topLeft)
            && _map.IsWalkable(topRight)
            && _map.IsWalkable(bottomLeft)
            && _map.IsWalkable(bottomRight);
    }

    private static Map CreateTestMap()
    {
        Map map = new(
            Guid.NewGuid(),
            "Test Map",
            15,
            13);

        for (int x = 0; x < map.Width; x++)
        {
            map.SetCell(
                new IndestructibleWall(
                    Guid.NewGuid(),
                    new GridPosition(x, 0)));

            map.SetCell(
                new IndestructibleWall(
                    Guid.NewGuid(),
                    new GridPosition(
                        x,
                        map.Height - 1)));
        }

        for (int y = 0; y < map.Height; y++)
        {
            map.SetCell(
                new IndestructibleWall(
                    Guid.NewGuid(),
                    new GridPosition(0, y)));

            map.SetCell(
                new IndestructibleWall(
                    Guid.NewGuid(),
                    new GridPosition(
                        map.Width - 1,
                        y)));
        }

        for (int x = 2; x < map.Width - 1; x += 2)
        {
            for (int y = 2; y < map.Height - 1; y += 2)
            {
                map.SetCell(
                    new IndestructibleWall(
                        Guid.NewGuid(),
                        new GridPosition(x, y)));
            }
        }

        map.SetCell(
            new DestructibleWall(
                Guid.NewGuid(),
                new GridPosition(3, 1)));

        map.SetCell(
            new DestructibleWall(
                Guid.NewGuid(),
                new GridPosition(4, 1)));

        map.SetCell(
            new DestructibleWall(
                Guid.NewGuid(),
                new GridPosition(5, 3)));

        map.SetCell(
            new DestructibleWall(
                Guid.NewGuid(),
                new GridPosition(7, 5)));

        map.SetCell(
            new DestructibleWall(
                Guid.NewGuid(),
                new GridPosition(9, 7)));

        map.AddSpawnPoint(
            new SpawnPoint(
                Guid.NewGuid(),
                new GridPosition(1, 1),
                0));

        map.AddSpawnPoint(
            new SpawnPoint(
                Guid.NewGuid(),
                new GridPosition(
                    map.Width - 2,
                    1),
                1));

        map.AddSpawnPoint(
            new SpawnPoint(
                Guid.NewGuid(),
                new GridPosition(
                    1,
                    map.Height - 2),
                2));

        map.AddSpawnPoint(
            new SpawnPoint(
                Guid.NewGuid(),
                new GridPosition(
                    map.Width - 2,
                    map.Height - 2),
                3));

        return map;
    }
}