using Bomber.Domain.Common;

namespace Bomber.Domain.Maps;

public sealed class DestructibleWall : Wall
{
    public bool IsDestroyed { get; private set; }

    public DestructibleWall(
        Guid id,
        GridPosition position)
        : base(id, position)
    {
    }

    public override bool IsWalkable => IsDestroyed;

    public override bool CanBeDestroyed => true;

    public override void Destroy()
    {
        IsDestroyed = true;
    }
}
