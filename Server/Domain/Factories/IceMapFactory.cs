using Bomber.Domain.Maps.Cells;

namespace Bomber.Domain.Factories;

public class IceMapFactory : IMapFactory
{
    public DestructibleWall CreateDestructibleWall()
    {
        return MapCellFlyweightFactory.Get<IceDestructibleWall>();
    }

    public Floor CreateFloor()
    {
        return MapCellFlyweightFactory.Get<IceFloor>();
    }

    public IndestructibleWall CreateIndestructibleWall()
    {
        return MapCellFlyweightFactory.Get<IceIndestructibleWall>();
    }

    public DestructibleWallSource CreateDestructibleWallSource()
    {
        return MapCellFlyweightFactory.Get<IceDestructibleWallSource>();
    }
}
