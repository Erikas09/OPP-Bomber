using Bomber.Domain.Common;
using Bomber.Domain.Maps.Cells;

namespace Bomber.Application.Factories;

public class IceMapFactory : IMapFactory
{
    public DestructibleWall CreateDestructibleWall(int x, int y)
    {
        return new IceDestructibleWall(Guid.NewGuid(), new GridPosition(x, y));
    }

    public Floor CreateFloor(int x, int y)
    {
        return new IceFloor(Guid.NewGuid(), new GridPosition(x,y));
    }

    public IndestructibleWall CreateIndestructibleWall(int x, int y)
    {
        return new IceIndestructibleWall(Guid.NewGuid(), new GridPosition(x, y));
    }
}
