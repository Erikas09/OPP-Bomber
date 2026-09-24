using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

public sealed class IndestructibleWall : Wall
{
    public IndestructibleWall(
        Guid id,
        int x,
        int y)
        : base(id, x, y)
    {
    }

    public override bool CanBeDestroyed => false;
}
