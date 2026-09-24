using Bomber.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Maps.Cells
{
    public class IceIndestructibleWall : IndestructibleWall
    {
        public IceIndestructibleWall(Guid id,GridPosition position): base(id, position){}
        public override bool CanBeDestroyed => true;
    }
}
