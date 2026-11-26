using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Factories;
using Bomber.Domain.PowerUps;

namespace Bomber.Domain.Maps.Cells;

public abstract class DestructibleWall : Wall, IDestructible
{
    public override bool CanBeDestroyed => true;

    public void Destroy(Map map)
    {
        GridPosition position = map.FindPosition(this);

        if (position.X < 0 || position.Y < 0)
            return;

        if (this is ClassicDestructibleWall)
            map.SetCell(position, MapCellFlyweightFactory.Get<ClassicFloor>());
        else
            map.SetCell(position, MapCellFlyweightFactory.Get<IceFloor>());

        int num = Random.Shared.Next(1, 4);

        PowerUpType type =
            num switch
            {
                1 => PowerUpType.ExtraBomb,
                2 => PowerUpType.BlastRadius,
                3 => PowerUpType.Speed,
                _ => throw new InvalidOperationException(),
            };

        PowerUp powerUp = PowerUpFactory.CreatePowerUp(type, position);
        map.AddPowerUp(powerUp);
    }
}
