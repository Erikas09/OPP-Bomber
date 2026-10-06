using Bomber.Domain.Common;
using Bomber.Domain.Players;

namespace Bomber.Domain.PowerUps;

public class BlastRadiusPowerUp : PowerUp
{
    int BlastRadius;
    public BlastRadiusPowerUp() {}

    public BlastRadiusPowerUp(int radius, GridPosition position)
    {
        Id = Guid.NewGuid();
        BlastRadius = radius;
        position_x = position.X;
        position_y = position.Y;
    }

    public override PowerUpPrototype Clone()
    {
        GridPosition pos = new GridPosition(position_x, position_y);
        return new BlastRadiusPowerUp(BlastRadius, pos);
    }
    public override void ApplyEffect(Player player)
    {
        int radius = player.GetExplosionRadius();
        player.SetExplosionRadius(radius + BlastRadius);
    }

}
