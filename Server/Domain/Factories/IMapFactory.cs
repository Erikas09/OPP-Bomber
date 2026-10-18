using Bomber.Domain.Maps.Cells;

namespace Bomber.Domain.Factories
{
    public interface IMapFactory
    {
        Floor CreateFloor(int x,int y);
        IndestructibleWall CreateIndestructibleWall(int x, int y);
        DestructibleWall CreateDestructibleWall(int x, int y);
    }
}
