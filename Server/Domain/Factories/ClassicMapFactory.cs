using Bomber.Domain.Maps.Cells;

namespace Bomber.Domain.Factories;

public class ClassicMapFactory : IMapFactory
{
    public DestructibleWall CreateDestructibleWall()
    {
        return MapCellFlyweightFactory.Get<ClassicDestructibleWall>();
    }

    public Floor CreateFloor()
    {
        return MapCellFlyweightFactory.Get<ClassicFloor>();
    }

    public IndestructibleWall CreateIndestructibleWall()
    {
        return MapCellFlyweightFactory.Get<ClassicIndestructibleWall>();
    }

    public DestructibleWallSource CreateDestructibleWallSource()
    {
        return MapCellFlyweightFactory.Get<ClassicDestructibleWallSource>();
    }
}
