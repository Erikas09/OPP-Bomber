using Bomber.Domain.Players;
using System.Windows.Input;

namespace Bomber.Domain.PowerUps;
public abstract class PowerUp : PowerUpPrototype
{
    public Guid Id;
    public int position_x;
    public int position_y;
    public abstract PowerUpPrototype Clone();
    public abstract void ApplyEffect(Player player);

}
