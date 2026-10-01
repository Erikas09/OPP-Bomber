namespace Bomber.Domain.PowerUps;

public class SpeedPowerUp : PowerUp
{
    public float SpeedModifier;

    public SpeedPowerUp() { }

    public SpeedPowerUp(float speedModifier)
    {
        SpeedModifier = speedModifier;
    }

    public override PowerUpPrototype Clone()
    {
        return new SpeedPowerUp(SpeedModifier);
    }
}
