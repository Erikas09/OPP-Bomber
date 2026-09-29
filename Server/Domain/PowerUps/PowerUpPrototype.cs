using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.PowerUps
{
    public interface PowerUpPrototype
    {
        public abstract PowerUpPrototype Clone();
    }
}
