namespace Bomber.Domain.Maps.Cells;

public abstract class Wall : MapCell
{
    public override bool IsWalkable => false;

    public abstract bool CanBeDestroyed { get; }
}
