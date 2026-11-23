using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Game
{
    public interface IGameObject
    {
        public void Update(float deltaTime);
    }
}
