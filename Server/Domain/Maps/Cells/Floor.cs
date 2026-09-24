namespace Bomber.Domain.Maps.Cells;

public class Floor : MapCell
{
    public Floor(
    Guid id,
    int x,
    int y)
    : base(id, x, y)
    {
    }

    public override bool IsWalkable => true;
}
