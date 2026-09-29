using Bomber.Domain.Common;
using Bomber.Domain.Maps.Cells;

namespace Bomber.Domain.Maps;

public sealed class Map
{
    private readonly MapCell[,] _cells;
    private readonly List<SpawnPoint> _spawnPoints = [];

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

    public bool IsWalkable(
        GridPosition position)
    {
        if (!IsInsideMap(position))
            return false;

        return _cells[position.X, position.Y].IsWalkable;
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
    }
}