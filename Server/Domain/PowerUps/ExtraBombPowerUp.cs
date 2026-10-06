using Bomber.Domain.Common;

namespace Bomber.Domain.PowerUps;

public class ExtraBombPowerUp : PowerUp
{
    public int BombCount;

    public ExtraBombPowerUp(){}

    public ExtraBombPowerUp(int bombCount, GridPosition position) 
    {
        Id = Guid.NewGuid();
        BombCount = bombCount;
        position_x = position.X;
        position_y = position.Y;
    }

    public override PowerUpPrototype Clone()
    {
        GridPosition pos = new GridPosition(position_x, position_y);
        return new ExtraBombPowerUp(BombCount, pos);
    }
}
