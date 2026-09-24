using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

public abstract class Wall : MapCell
{
    protected Wall(
        Guid id,
        int x,
        int y)
        : base(id, x, y)
    {
    }

    public override bool IsWalkable => false;

    public abstract bool CanBeDestroyed { get; }
}
