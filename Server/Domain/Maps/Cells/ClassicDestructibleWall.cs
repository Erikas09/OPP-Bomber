using Bomber.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Maps.Cells
{
    public class ClassicDestructibleWall : DestructibleWall
    {
        public ClassicDestructibleWall(Guid id, GridPosition position) : base(id, position) { }
    }
}
