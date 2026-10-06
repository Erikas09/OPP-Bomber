using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Entities.Bots
{
    public class Bot : Player
    {
        BotStrategy strategy;

        public Bot(Guid id, string name, Position position, int lives, int bombLimit, int explosionRadius, float speed) :
            base(id, name, position, lives, bombLimit, explosionRadius, speed)
        {
            strategy = new HuntStrategy();
        }

        public void SetStrategy(BotStrategy strategy)
        {
            strategy.SetBot(this);
            this.strategy = strategy;
        }

        public void Update(float deltaTime)
        {
            strategy.Update(deltaTime);
        }
    }
}
