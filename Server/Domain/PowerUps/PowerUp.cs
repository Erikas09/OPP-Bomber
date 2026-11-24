using Bomber.Domain.Common;
using Bomber.Domain.Players;
using System.Windows.Input;

namespace Bomber.Domain.PowerUps;
public abstract class PowerUp : PowerUpPrototype
{
    public Guid Id;
    public int position_x;
    public int position_y;
    public abstract PowerUpPrototype Clone();
    public bool ApplyEffect(Player player)
    {
        if (!PlayerIsValid(player))
            return false;

        if (!PlayerIsAlive(player))
            return false;

        ApplySpecificEffect(player);
        return true;

    }
    public virtual bool PlayerIsValid(Player player)
    {
        if (player == null)
            return false;
        
        return true;
    }
    public virtual bool PlayerIsAlive(Player player)
    {
        return player.State != Enums.PlayerState.Dead;
    }
    public abstract void ApplySpecificEffect(Player player);

    public void Move(GridPosition pos)
    {
        position_x = pos.X;
        position_y = pos.Y;
    }

}
