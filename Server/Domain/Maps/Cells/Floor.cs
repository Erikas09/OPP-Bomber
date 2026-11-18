namespace Bomber.Domain.Maps.Cells;

public abstract class Floor : MapCell
{
    public override bool IsWalkable => true;

    // Read-only after construction: this object is shared by many tiles (Flyweight).
    public float walkingSpeedModifier { get; protected init; } = 1f;
}
