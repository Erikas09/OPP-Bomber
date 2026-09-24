using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

public sealed class IndestructibleWall : Wall
{
    public IndestructibleWall(
        Guid id,
        GridPosition position)
        : base(id, position)
    {
    }

    public override bool CanBeDestroyed => false;
}
