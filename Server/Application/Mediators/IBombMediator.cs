using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Application.Mediators
{
    public interface IBombMediator
    {
        void HandleExplosion(IBomb bomb, Explosion explosion, List<GridPosition> affectedCells);
    }
}
