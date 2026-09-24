using Bomber.Domain.Common;

namespace Bomber.Domain.Maps;

public abstract class Wall
{
    public Guid Id { get; }

    public GridPosition Position { get; }

    protected Wall(
        Guid id,
        GridPosition position)
    {
        Id = id;
        Position = position;
    }

    public abstract bool IsWalkable { get; }

    public abstract bool CanBeDestroyed { get; }

    public abstract void Destroy();
}
