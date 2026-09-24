using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

public sealed class DestructibleWall : Wall
{
    public DestructibleWall(
        Guid id,
        GridPosition position)
        : base(id, position)
    {
    }

    public override bool CanBeDestroyed => true;
}
