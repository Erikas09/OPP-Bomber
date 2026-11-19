using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Game
{
    public class GameWorld : IGameObject
    {
        private readonly List<IGameObject> _gameObjects = [];

        public void Add(IGameObject component)
        {
            _gameObjects.Add(component);
        }

        public void Update(float deltaTime)
        {
            foreach (IGameObject gameObject in _gameObjects)
            {
                gameObject.Update(deltaTime);
            }
        }
    }
}
