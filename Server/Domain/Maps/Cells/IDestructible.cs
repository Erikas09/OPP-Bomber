using Bomber.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Maps.Cells
{
    public interface IDestructible
    {
        public void Destroy(Map map);
    }
}
