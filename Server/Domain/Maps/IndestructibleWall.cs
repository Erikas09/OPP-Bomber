using Bomber.Domain.Common;

namespace Bomber.Domain.Maps;

public sealed class IndestructibleWall : Wall
{
    public IndestructibleWall(
    Guid id,
    GridPosition position)
    : base(id, position)
    {
    }

    public override bool IsWalkable => false;

    public override bool CanBeDestroyed => false;

    public override void Destroy()
    {
    }
}
