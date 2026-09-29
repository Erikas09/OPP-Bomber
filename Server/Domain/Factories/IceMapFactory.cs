using Bomber.Domain.Maps.Cells;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Factories
{
    public class IceMapFactory : IMapFactory
    {
        public DestructibleWall CreateDestructibleWall(int x, int y)
        {
            return new IceDestructibleWall(Guid.NewGuid(), new Common.GridPosition(x, y));
        }

        public Floor CreateFloor(int x, int y)
        {
            return new IceFloor(Guid.NewGuid(), new Common.GridPosition(x,y));
        }

        public IndestructibleWall CreateIndestructibleWall(int x, int y)
        {
            return new IceIndestructibleWall(Guid.NewGuid(), new Common.GridPosition(x, y));
        }
    }
}
