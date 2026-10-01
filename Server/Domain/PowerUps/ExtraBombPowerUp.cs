namespace Bomber.Domain.PowerUps;

public class ExtraBombPowerUp : PowerUp
{
    public int BombCount;

    public ExtraBombPowerUp(){}

    public ExtraBombPowerUp(int bombCount) 
    {
        BombCount = bombCount;
    }

    public override PowerUpPrototype Clone()
    {
        return new ExtraBombPowerUp(BombCount);
    }
}
