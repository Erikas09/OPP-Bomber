using Bomber.Domain.Common;

namespace Bomber.Domain.PowerUps;

public class BlastRadiusPowerUp : PowerUp
{
    float BlastRadius;
    public BlastRadiusPowerUp() {}

    public BlastRadiusPowerUp(float radius, GridPosition position)
    {
        Id = Guid.NewGuid();
        BlastRadius = radius;
        position_x = position.X;
        position_y = position.Y;
    }

    public override PowerUpPrototype Clone()
    {
        GridPosition pos = new GridPosition(position_x, position_y);
        return new BlastRadiusPowerUp(BlastRadius, pos);
    }

}
