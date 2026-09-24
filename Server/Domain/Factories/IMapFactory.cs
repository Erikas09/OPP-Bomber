using System;
using System.Collections.Generic;
using System.Text;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;

namespace Bomber.Domain.Factories
{
    interface IMapFactory
    {
        Floor CreateFloor(int x,int y);
        IndestructibleWall CreateIndestructibleWall(int x, int y);
        DestructibleWall CreateDestructibleWall(int x, int y);
    }
}
