namespace Bomber.Domain.PowerUps;

public class BlastRadiusPowerUp : PowerUp
{
    float blastRadius;
    public BlastRadiusPowerUp() {}

    public BlastRadiusPowerUp(float radius)
    {
        this.blastRadius = radius;
    }

    public override PowerUpPrototype Clone()
    {
        return new BlastRadiusPowerUp(blastRadius);
    }

}
