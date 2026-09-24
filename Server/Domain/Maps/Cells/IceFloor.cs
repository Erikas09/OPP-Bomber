using Bomber.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Maps.Cells
{
    public class IceFloor : Floor
    {
        public IceFloor(Guid id, GridPosition position) : base(id, position) { }
    }
}
