using Bomber.Domain.Common;

namespace Bomber.Domain.PowerUps;

public class SpeedPowerUp : PowerUp
{
    public float SpeedModifier;

    public SpeedPowerUp() { }

    public SpeedPowerUp(float speedModifier, GridPosition position)
    {
        Id = Guid.NewGuid();
        SpeedModifier = speedModifier;
        position_x = position.X;
        position_y = position.Y;
    }

    public override PowerUpPrototype Clone()
    {
        GridPosition pos = new GridPosition(position_x, position_y);
        return new SpeedPowerUp(SpeedModifier, pos);
    }
}
