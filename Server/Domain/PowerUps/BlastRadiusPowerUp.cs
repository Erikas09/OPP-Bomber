namespace Bomber.Domain.PowerUps;

public class BlastRadiusPowerUp : PowerUp
{
    float BlastRadius;
    public BlastRadiusPowerUp() {}

    public BlastRadiusPowerUp(float radius)
    {
        BlastRadius = radius;
    }

    public override PowerUpPrototype Clone()
    {
        return new BlastRadiusPowerUp(BlastRadius);
    }

}
