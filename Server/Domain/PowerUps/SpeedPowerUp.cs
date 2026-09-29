namespace Bomber.Domain.PowerUps;

public class SpeedPowerUp : PowerUp
{
    public float speedModifier;

    public SpeedPowerUp() { }

    public SpeedPowerUp(float speedModifier)
    {
        this.speedModifier = speedModifier;
    }

    public override PowerUpPrototype Clone()
    {
        return new SpeedPowerUp(speedModifier);
    }
}
