using Bomber.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Maps.Cells
{
    internal class IceDestructibleWall : DestructibleWall
    {
        public IceDestructibleWall(Guid id, GridPosition position) : base(id, position) { }
    }
}
