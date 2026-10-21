using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Factories;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using Bomber.Domain.PowerUps;

namespace Bomber.Domain.Maps;

public sealed class Map
{
    public enum MapType
    {
        Classic,
        Ice
    }

    private const int TileSize = 40;
    private readonly MapCell[,] _cells;
    private readonly List<SpawnPoint> _spawnPoints = [];
    private readonly List<PowerUp> _powerUps;

    public Guid Id { get; }
    public string Name { get; }
    public int Width { get; }
    public int Height { get; }

    public List<SpawnPoint> SpawnPoints => _spawnPoints;

    public Map(
        Guid id,
        string name,
        int width,
        int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        Id = id;
        Name = name;
        Width = width;
        Height = height;

        _cells = new MapCell[width, height];
        
        _powerUps = new List<PowerUp>();
    }

    public MapCell GetCell(
        GridPosition position)
    {
        if (!IsInsideMap(position))
            throw new ArgumentOutOfRangeException(nameof(position));

        return _cells[position.X, position.Y];
    }

    public void SetCell(MapCell cell)
    {
        if (!IsInsideMap(cell.Position))
            throw new ArgumentOutOfRangeException(nameof(cell));

        _cells[cell.Position.X, cell.Position.Y] = cell;
    }

    public List<GridPosition> GetAllWalkablePositions()
    {
        List<GridPosition> positions = new();

        for(int x = 0; x < Width; x++)
            for(int y = 0; y < Height; y++)
            {
                GridPosition position = new(x, y);
                if (IsWalkable(position))
                    positions.Add(position);
            }

        return positions;
    }

    public bool IsWalkable(
        GridPosition position)
    {
        if (!IsInsideMap(position))
            return false;

        return _cells[position.X, position.Y].IsWalkable;
    }

    public bool IsWalkable(Position position, float size)
    {
        int left =
            (int)(position.X / TileSize);

        int right =
            (int)((position.X + size - 1) / TileSize);

        int top =
            (int)(position.Y / TileSize);

        int bottom =
            (int)((position.Y + size - 1) / TileSize);

        GridPosition topLeft =
            new(left, top);

        GridPosition topRight =
            new(right, top);

        GridPosition bottomLeft =
            new(left, bottom);

        GridPosition bottomRight =
            new(right, bottom);

        return IsWalkable(topLeft)
            && IsWalkable(topRight)
            && IsWalkable(bottomLeft)
            && IsWalkable(bottomRight);
    }

    public bool IsInsideMap(
        GridPosition position)
    {
        return IsInsideMap(position.X, position.Y);
    }

    public bool IsInsideMap(
        int x,
        int y)
    {
        return x >= 0 &&
               y >= 0 &&
               x < Width &&
               y < Height;
    }

    public void AddSpawnPoint(
        SpawnPoint spawnPoint)
    {
        if (!IsInsideMap(spawnPoint.Position))
            throw new ArgumentException("Spawn point must be inside the map.", nameof(spawnPoint));

        _spawnPoints.Add(spawnPoint);
    }

    public SpawnPoint? GetSpawnPoint(
        int playerIndex)
    {
        foreach (SpawnPoint spawnPoint in _spawnPoints)
            if (spawnPoint.PlayerIndex == playerIndex)
                return spawnPoint;

        return null;
    }

    public List<SpawnPoint> GetNewSpawnPoints(int playerCount, int count)
    {
        List<SpawnPoint> points = new();

        for (int i = 0; i < count; i++)
        {
            if (_spawnPoints.Count > playerCount + i)
                points.Add(_spawnPoints[playerCount + i]);
        }

        return points;
    }

    public List<PowerUp> GetPowerUps()
    {
        return _powerUps;
    }
    
    public PowerUp? GetPowerUpByPosition(GridPosition position)
    {
        foreach(PowerUp powerUp in _powerUps)
        {
            if (powerUp.position_x == position.X && powerUp.position_y == position.Y)
                return powerUp;
        }

        return null;
    }
    public void RemovePowerUp(GridPosition position)
    {
        PowerUp? powerUp = GetPowerUpByPosition(position);

        if (powerUp is not null)
            _powerUps.Remove(powerUp);
    }
    public void AddPowerUp(PowerUp powerUp)
    {
        _powerUps.Add(powerUp);
    }

    public GridPosition GetRandomValidPosition()
    {
        while (true)
        {
            int x = Random.Shared.Next(Width);
            int y = Random.Shared.Next(Height);

            GridPosition position = new GridPosition(x, y);

            if (!IsWalkable(position))
                continue;

            if (_cells[x, y] is not Floor)
                continue;

            if (GetPowerUpByPosition(position) is not null)
                continue;

            return position;
        }
    }

    public void DestroyWall(
        GridPosition position)
    {
        if (!IsInsideMap(position))
            return;

        MapCell cell = GetCell(position);

        if (cell is not DestructibleWall)
            return;

        if (cell is ClassicDestructibleWall)
            SetCell(new ClassicFloor(Guid.NewGuid(), position));
        else
            SetCell(new IceFloor(Guid.NewGuid(), position));

        int num = Random.Shared.Next(1, 4);

        PowerUpType type =
            num switch
            {
                1 => PowerUpType.ExtraBomb,
                2 => PowerUpType.BlastRadius,
                3 => PowerUpType.Speed,
                _ => throw new InvalidOperationException(),
            };

        PowerUp powerUp = PowerUpFactory.CreatePowerUp(type, position);

        _powerUps.Add(powerUp);
    }

    public static GridPosition WorldToGridPosition(Position position, float size)
    {
        float centerX = position.X + size / 2f;
        float centerY = position.Y + size / 2f;

        int gridX = (int)(centerX / TileSize);
        int gridY = (int)(centerY / TileSize);

        return new GridPosition(gridX, gridY);
    }

    public static Position GridToWorldPosition(GridPosition position, float size)
    {
        return new(
            (position.X + 0.5f) * TileSize - size * 0.5f,
            (position.Y + 0.5f) * TileSize - size * 0.5f);
    }

    public static Map CreateTestMap(MapType type)
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
}