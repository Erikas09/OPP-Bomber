using Bomber.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Maps.Cells
{
    public class ClassicFloor : Floor
    {
        public ClassicFloor(Guid id, GridPosition position) : base(id, position) 
        {
            walkingSpeedModifier = 1;
        }
    }
}
