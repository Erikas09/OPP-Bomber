using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

public abstract class Wall : MapCell
{
    protected Wall(
        Guid id,
        GridPosition position)
        : base(id, position)
    {
    }

    public override bool IsWalkable => false;

    public abstract bool CanBeDestroyed { get; }
}
