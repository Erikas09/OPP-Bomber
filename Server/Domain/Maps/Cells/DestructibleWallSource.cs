using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Factories;
using Bomber.Domain.PowerUps;

namespace Bomber.Domain.Maps.Cells;

public abstract class DestructibleWallSource : Wall, IDestructible
{
    public override bool CanBeDestroyed => true;

    List<IDestructible> connectedWalls = new();


    public void AddWall(IDestructible wall)
    {
        connectedWalls.Add(wall);
    }

    public void Destroy(Map map)
    {
        GridPosition position = map.FindPosition(this);

        if (position.X < 0 || position.Y < 0)
            return;

        if (this is ClassicDestructibleWallSource)
            map.SetCell(position, MapCellFlyweightFactory.Get<ClassicFloor>());
        else
            map.SetCell(position, MapCellFlyweightFactory.Get<IceFloor>());

        foreach (var wall in connectedWalls)
            wall.Destroy(map);
    }
}
