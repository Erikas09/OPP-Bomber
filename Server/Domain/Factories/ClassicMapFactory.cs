using Bomber.Domain.Common;
using Bomber.Domain.Maps.Cells;

namespace Bomber.Domain.Factories;

public class ClassicMapFactory : IMapFactory
{
    public DestructibleWall CreateDestructibleWall(int x, int y)
    {
        return new ClassicDestructibleWall(Guid.NewGuid(), new GridPosition(x, y));

    }

    public Floor CreateFloor(int x, int y)
    {
        return new ClassicFloor(Guid.NewGuid(), new GridPosition(x, y));
    }

    public IndestructibleWall CreateIndestructibleWall(int x, int y)
    {
        return new ClassicIndestructibleWall(Guid.NewGuid(), new GridPosition(x, y));
    }
}
