namespace Bomber.Domain.Maps.Cells;

public abstract class DestructibleWall : Wall
{
    public override bool CanBeDestroyed => true;
}
