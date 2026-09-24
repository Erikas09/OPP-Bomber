using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

public sealed class DestructibleWall : Wall
{
    public DestructibleWall(
        Guid id,
        int x,
        int y)
        : base(id, x, y)
    {
    }

    public override bool CanBeDestroyed => true;
}
