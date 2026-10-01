using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Entities.Bots
{
    public class TestBotStrategy : BotStrategy
    {
        const int STRATEGY_UPDATE_FREQUENCY = 20;

        Random rng;
        int strategy_updates = 0;


        public TestBotStrategy(BotManager manager) : base(manager)
        {
            rng = new();
        }

        public override void Update(Bot bot, float deltaTime)
        {
            strategy_updates++;

            if (strategy_updates >= STRATEGY_UPDATE_FREQUENCY)
            {
                strategy_updates = 0;

                int directionX = rng.Next(3) - 1;
                int directionY = rng.Next(3) - 1;

                bot.SetInput(new(directionY == 1, directionY == -1, directionX == 1, directionX == -1));
            }
        }
    }
}
