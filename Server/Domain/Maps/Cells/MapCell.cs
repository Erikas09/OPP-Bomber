namespace Bomber.Domain.Maps.Cells;

public abstract class MapCell
{
    public Guid Id { get; }
    public int X { get; }
    public int Y { get; }
    public abstract bool IsWalkable { get; }

    protected MapCell(
        Guid id,
        int x,
        int y)
    {
        Id = id;
        X = x;
        Y = y;
    }
}
