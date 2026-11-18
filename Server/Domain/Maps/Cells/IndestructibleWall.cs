namespace Bomber.Domain.Maps.Cells;

public abstract class IndestructibleWall : Wall
{
    public override bool CanBeDestroyed => false;
}
