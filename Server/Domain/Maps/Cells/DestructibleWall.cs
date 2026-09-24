using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

public abstract class DestructibleWall : Wall
{
    public DestructibleWall(
        Guid id,
        GridPosition position)
        : base(id, position)
    {
    }
    public override bool CanBeDestroyed => true;
}
