using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

public abstract class MapCell
{
    public Guid Id { get; }
    public GridPosition Position { get; }
    public abstract bool IsWalkable { get; }

    protected MapCell(
        Guid id,
        GridPosition position)
    {
        Id = id;
        Position = position;
    }
}
