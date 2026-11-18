using Bomber.Domain.Maps.Cells;

namespace Bomber.Domain.Factories
{
    // Abstract Factory: creates a family (Classic / Ice) of map cells.
    // Cells are shared flyweights, so no position is passed here;
    // the position (extrinsic state) is stored in MapTile.
    public interface IMapFactory
    {
        Floor CreateFloor();
        IndestructibleWall CreateIndestructibleWall();
        DestructibleWall CreateDestructibleWall();
    }
}
