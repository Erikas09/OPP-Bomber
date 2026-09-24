using Bomber.Domain.Common;

namespace Bomber.Domain.Maps.Cells;

public class Floor : MapCell
{
    public Floor(
    Guid id,
    GridPosition position)
    : base(id, position)
    {
    }

    public override bool IsWalkable => true;
}
