using Bomber.Domain.Maps.Cells;

namespace Bomber.Domain.Maps;

public sealed class Map
{
    private readonly MapCell[,] _cells;

    public Guid Id { get; }

    public string Name { get; }

    public int Width { get; }

    public int Height { get; }

    public Map(
        Guid id,
        string name,
        int width,
        int height)
    {
        Id = id;
        Name = name;
        Width = width;
        Height = height;

        _cells = new MapCell[width, height];
    }

    public MapCell GetCell(int x, int y)
    {
        return _cells[x, y];
    }

    public void SetCell(int x, int y, MapCell cell)
    {
        _cells[x, y] = cell;
    }

    public bool IsWalkable(int x, int y)
    {
        if (!IsInsideMap(x, y))
            return false;

        return _cells[x, y].IsWalkable;
    }

    public bool IsInsideMap(int x, int y)
    {
        return x >= 0 &&
               y >= 0 &&
               x < Width &&
               y < Height;
    }
}
