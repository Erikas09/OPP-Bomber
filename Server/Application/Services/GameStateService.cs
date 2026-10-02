using Bomber.Application.Factories;
using Bomber.Application.Publishers;
using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;
using Bomber.Domain.Enums;
using Bomber.Domain.Events;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;

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
    private readonly GameEventPublisher _eventPublisher;

    public GameStateService(GameEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;

        _map = CreateTestMap(MapType.Classic);
        BotManager.Initialize(_map, _players);
        _botManager = BotManager.Instance;

        for (int i = 0; i < 2; i++)
        {
            SpawnPoint spawnPoint =
            _map.SpawnPoints.ElementAt(_players.Count);

            Position position = new(
                spawnPoint.Position.X * TileSize,
                spawnPoint.Position.Y * TileSize);

            Bot bot;

            if (i == 0)
                bot = _botManager.CreateHunter(position);
            else
                bot = _botManager.CreateDefender(position);

            AddPlayer(bot);
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

            _botManager.Update(deltaTime);
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

        float length = MathF.Sqrt(
            movementX * movementX +
            movementY * movementY);

        if (length > 0f)
        {
            movementX /= length;
            movementY /= length;
        }

        float walkingSpeedModifier = 1;
        MapCell cell = _map.GetCell(GetPlayerGridPosition(player));

        if (cell is ClassicFloor floor)
        {
            walkingSpeedModifier = floor.walkingSpeedModifier;
            float distance = player.Speed * deltaTime * walkingSpeedModifier;

            movementX *= distance;
            movementY *= distance;
        }
        else if (cell is IceFloor ifloor)
        {
            walkingSpeedModifier = ifloor.walkingSpeedModifier;
            float targetVelocityX = movementX * player.Speed * walkingSpeedModifier;
            float targetVelocityY = movementY * player.Speed * walkingSpeedModifier;

            float maxDelta = 2 * deltaTime;
            player.VelocityX = MoveToward(player.VelocityX, targetVelocityX, maxDelta * player.Speed);
            player.VelocityY = MoveToward(player.VelocityY, targetVelocityY, maxDelta * player.Speed);

            movementX = player.VelocityX * deltaTime;
            movementY = player.VelocityY * deltaTime;
        }

        Position horizontalPosition = new(
            player.Position.X + movementX,
            player.Position.Y);

        if (CanPlayerMoveTo(horizontalPosition))
        {
            player.MoveTo(horizontalPosition);
        }
        else
            player.VelocityX = 0f;

        Position verticalPosition = new(
            player.Position.X,
            player.Position.Y + movementY);

        if (CanPlayerMoveTo(verticalPosition))
        {
            player.MoveTo(verticalPosition);
        }
        else
            player.VelocityY = 0f;
    }
    private static float MoveToward(float current, float target, float maxDelta)
    {
        float diff = target - current;
        if (MathF.Abs(diff) <= maxDelta)
            return target;
        return current + MathF.Sign(diff) * maxDelta;
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

    private enum MapType
    {
        Classic,
        Ice
    }

    private static Map CreateTestMap(MapType type)
    {
        Map map = new(
            Guid.NewGuid(),
            "Test Map",
            15,
            13);

        IMapFactory factory;

        if (type == MapType.Classic)
            factory = new ClassicMapFactory();
        else
            factory = new IceMapFactory();


        for (int x = 0; x < map.Width; x++)
            for (int y = 0; y < map.Height; y++)
                map.SetCell(factory.CreateFloor(x, y));

        for (int x = 0; x < map.Width; x++)
        {
            map.SetCell(factory.CreateIndestructibleWall(x, 0));
            map.SetCell(factory.CreateIndestructibleWall(x, map.Height - 1));
        }

        for (int y = 0; y < map.Height; y++)
        {
            map.SetCell(factory.CreateIndestructibleWall(0, y));
            map.SetCell(factory.CreateIndestructibleWall(map.Width - 1, y));
        }

        for (int x = 2; x < map.Width - 1; x += 2)
            for (int y = 2; y < map.Height - 1; y += 2)
                map.SetCell(factory.CreateIndestructibleWall(x, y));

        map.SetCell(factory.CreateDestructibleWall(3, 1));
        map.SetCell(factory.CreateDestructibleWall(4, 1));
        map.SetCell(factory.CreateDestructibleWall(5, 3));
        map.SetCell(factory.CreateDestructibleWall(7, 5));
        map.SetCell(factory.CreateDestructibleWall(9, 7));

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
        List<GameEvent> events = [];
        List<GridPosition> affectedCells;

        lock (_lock)
        {
            bool found = _bombs.TryGetValue(bombId, out Bomb? bomb);

            if (!found || bomb is null)
                return [];

            _bombs.Remove(bombId);

            affectedCells = ComputeExplosionCells(bomb.Position, bomb.Radius);

            DateTimeOffset occurredAt = DateTimeOffset.UtcNow;

            events.Add(
                new BombExplodedEvent(
                    Guid.NewGuid(),
                    occurredAt,
                    bomb.Id,
                    bomb.OwnerId,
                    bomb.Position,
                    [.. affectedCells]));

            foreach (GridPosition cell in affectedCells)
            {
                MapCell mapCell = _map.GetCell(cell);

                if (mapCell is not DestructibleWall)
                    continue;

                Guid wallId = mapCell.Id;

                _map.DestroyWall(cell);

                events.Add(
                    new WallDestroyedEvent(
                        Guid.NewGuid(),
                        occurredAt,
                        wallId,
                        bomb.Id,
                        bomb.OwnerId,
                        cell));
            }

            foreach (Player player in _players.Values)
            {
                if (player.State == PlayerState.Dead)
                    continue;

                GridPosition playerCell = GetPlayerGridPosition(player);

                if (!affectedCells.Contains(playerCell))
                    continue;

                int livesBefore = player.Lives;
                PlayerState stateBefore = player.State;

                player.TakeDamage();

                if (player.Lives < livesBefore)
                    events.Add(
                        new PlayerDamagedEvent(
                            Guid.NewGuid(),
                            occurredAt,
                            player.Id,
                            bomb.Id,
                            bomb.OwnerId,
                            player.Lives));

                if (stateBefore != PlayerState.Dead && player.State == PlayerState.Dead)
                    events.Add(
                        new PlayerDiedEvent(
                            Guid.NewGuid(),
                            occurredAt,
                            player.Id,
                            bomb.Id,
                            bomb.OwnerId));
            }
        }

        foreach (GameEvent gameEvent in events)
            _eventPublisher.Notify(gameEvent);

        return affectedCells;
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