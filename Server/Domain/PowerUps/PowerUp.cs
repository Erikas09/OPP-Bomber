namespace Bomber.Domain.PowerUps;

public abstract class PowerUp : PowerUpPrototype
{
    public abstract PowerUpPrototype Clone();
}
