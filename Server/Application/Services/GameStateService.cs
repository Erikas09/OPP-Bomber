using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;
using Bomber.Domain.Enums;
using Bomber.Domain.Factories;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using System.Diagnostics;

namespace Bomber.Application.Services;

public sealed class GameStateService
{
    private const float PlayerSize = 30f;
    private const int TileSize = 40;

    private readonly Lock _lock = new();

    private readonly Dictionary<Guid, Player> _players = [];
    private readonly Dictionary<Guid, Bomb> _bombs = [];

    private readonly Map _map;
    private readonly BotManager _botManager;

    public GameStateService()
    {
        _map = CreateTestMap();
        _botManager = new BotManager();

        for(int i = 0; i < 2; i++)
        {
            SpawnPoint spawnPoint = _map.SpawnPoints.ElementAt(_players.Count);

            Position position = new(
                spawnPoint.Position.X * TileSize,
                spawnPoint.Position.Y * TileSize);

            AddPlayer(_botManager.CreateBot(position));
        }
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
                throw new InvalidOperationException(
                    "No spawn point is available.");

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

            _players.Add(playerId, player);
        }
    }

    public void AddPlayer(Player player)
    {
        _players.Add(player.Id, player);
    }

    public void RemovePlayer(Guid playerId)
    {
        lock (_lock)
        {
            _players.Remove(playerId);
        }
    }

    public void SetPlayerInput(
        Guid playerId,
        PlayerInput input)
    {
        lock (_lock)
        {
            if (!_players.ContainsKey(playerId))
                return;

            _players[playerId].SetInput(input);
        }
    }

    public void Update(float deltaTime)
    {
        lock (_lock)
        {
            foreach (Player player in _players.Values)
            {
                if (player.State == PlayerState.Dead)
                    continue;

                UpdatePlayer(player, player.Input, deltaTime);
            }

            _botManager.Update();
        }
    }

    public List<Player> GetPlayers()
    {
        lock (_lock)
        {
            return [.. _players.Values];
        }
    }

    public Map GetMap()
    {
        return _map;
    }

    private void UpdatePlayer(
        Player player,
        PlayerInput input,
        float deltaTime)
    {
        float movementX = 0f;
        float movementY = 0f;

        if (input.Up)
            movementY -= 1f;

        if (input.Down)
            movementY += 1f;

        if (input.Left)
            movementX -= 1f;

        if (input.Right)
            movementX += 1f;

        if (movementX == 0f && movementY == 0f)
            return;

        // Normalize diagonal movement.
        float length = MathF.Sqrt(
            movementX * movementX +
            movementY * movementY);

        movementX /= length;
        movementY /= length;

        float distance =
            player.Speed * deltaTime;

        movementX *= distance;
        movementY *= distance;

        // Resolve horizontal movement separately.
        Position horizontalPosition = new(
            player.Position.X + movementX,
            player.Position.Y);

        if (CanPlayerMoveTo(horizontalPosition))
        {
            player.MoveTo(horizontalPosition);
        }

        // Resolve vertical movement separately.
        Position verticalPosition = new(
            player.Position.X,
            player.Position.Y + movementY);

        if (CanPlayerMoveTo(verticalPosition))
        {
            player.MoveTo(verticalPosition);
        }
    }

    private bool CanPlayerMoveTo(Position position)
    {
        int left =
            (int)(position.X / TileSize);

        int right =
            (int)((position.X + PlayerSize - 1) / TileSize);

        int top =
            (int)(position.Y / TileSize);

        int bottom =
            (int)((position.Y + PlayerSize - 1) / TileSize);

        GridPosition topLeft =
            new(left, top);

        GridPosition topRight =
            new(right, top);

        GridPosition bottomLeft =
            new(left, bottom);

        GridPosition bottomRight =
            new(right, bottom);

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

        var classicFactory = new ClassicMapFactory();


        for (int x = 0; x < map.Width; x++)
        {
            map.SetCell(classicFactory.CreateIndestructibleWall(x, 0));
            map.SetCell(classicFactory.CreateIndestructibleWall(x, map.Height - 1));
        }

        for (int y = 0; y < map.Height; y++)
        {
            map.SetCell(classicFactory.CreateIndestructibleWall(0, y));
            map.SetCell(classicFactory.CreateIndestructibleWall(map.Width - 1, y));
        }

        for (int x = 2; x < map.Width - 1; x += 2)
        {
            for (int y = 2; y < map.Height - 1; y += 2)
            {
                map.SetCell(classicFactory.CreateIndestructibleWall(x,y));
            }
        }

        map.SetCell(classicFactory.CreateDestructibleWall(3, 1));
        map.SetCell(classicFactory.CreateDestructibleWall(4, 1));
        map.SetCell(classicFactory.CreateDestructibleWall(5, 3));
        map.SetCell(classicFactory.CreateDestructibleWall(7, 5));
        map.SetCell(classicFactory.CreateDestructibleWall(9, 7));

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
    public bool PlaceBomb(
    Guid playerId,
    out Bomb? bomb)
    {
        lock (_lock)
        {
            bomb = null;

            bool found = _players.TryGetValue(playerId, out Player? player);

            if (!found || player is null)
                return false;

            if (player.State == PlayerState.Dead)
                return false;

            int activeBombs = _bombs.Values.Count(b => b.OwnerId == playerId);

            if (activeBombs >= player.BombLimit)
                return false;

            GridPosition position = GetPlayerGridPosition(player);

            if (!_map.IsWalkable(position))
                return false;

            bool occupied = _bombs.Values.Any(b => b.Position == position);

            if (occupied)
                return false;

            bomb = new Bomb(
                Guid.NewGuid(),
                playerId,
                position,
                player.ExplosionRadius,
                fuseSeconds: 3);

            _bombs.Add(bomb.Id, bomb);

            return true;
        }
    }
    private GridPosition GetPlayerGridPosition(Player player)
    {
        float centerX = player.Position.X + PlayerSize / 2f;
        float centerY = player.Position.Y + PlayerSize / 2f;

        int gridX = (int)(centerX / TileSize);
        int gridY = (int)(centerY / TileSize);

        return new GridPosition(gridX, gridY);
    }
    private List<GridPosition> ComputeExplosionCells(
    GridPosition origin,
    int radius)
    {
        List<GridPosition> cells = [origin];

        (int dx, int dy)[] directions =
        [
            (0, -1),
        (0, 1),
        (-1, 0),
        (1, 0),
    ];

        foreach ((int dx, int dy) in directions)
        {
            for (int step = 1; step <= radius; step++)
            {
                GridPosition current = new(
                    origin.X + dx * step,
                    origin.Y + dy * step);

                if (!_map.IsInsideMap(current))
                    break;

                MapCell cell = _map.GetCell(current);

                if (cell is IndestructibleWall)
                    break;

                cells.Add(current);

                if (cell is DestructibleWall)
                    break;
            }
        }

        return cells;
    }
    public List<GridPosition> ExplodeBomb(Guid bombId)
    {
        lock (_lock)
        {
            bool found = _bombs.TryGetValue(bombId, out Bomb? bomb);

            if (!found || bomb is null)
                return [];

            _bombs.Remove(bombId);

            List<GridPosition> affectedCells = ComputeExplosionCells(bomb.Position, bomb.Radius);

            foreach (GridPosition cell in affectedCells)
                _map.DestroyWall(cell);

            foreach (Player player in _players.Values)
            {
                if (player.State == PlayerState.Dead)
                    continue;

                GridPosition playerCell = GetPlayerGridPosition(player);

                if (affectedCells.Contains(playerCell))
                    player.TakeDamage();
            }

            return affectedCells;
        }
    }
    public List<Bomb> GetBombsToExplode(DateTimeOffset now)
    {
        lock (_lock)
            return [.. _bombs.Values.Where(b => b.ShouldExplode(now))];
    }

    public List<Bomb> GetBombs()
    {
        lock (_lock)
            return [.. _bombs.Values];
    }


}