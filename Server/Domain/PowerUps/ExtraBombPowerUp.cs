namespace Bomber.Domain.PowerUps;

public class ExtraBombPowerUp : PowerUp
{
    public int bombCount;

    public ExtraBombPowerUp(){}

    public ExtraBombPowerUp(int bombCount) 
    {
        this.bombCount = bombCount;
    }

    public override PowerUpPrototype Clone()
    {
        return new ExtraBombPowerUp(bombCount);
    }
}
