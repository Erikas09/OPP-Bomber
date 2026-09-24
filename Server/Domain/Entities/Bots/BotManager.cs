using Bomber.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Entities.Bots
{
    public class BotManager
    {
        const int STRATEGY_UPDATE_FREQUENCY = 200;

        Dictionary<Guid, Bot> bots = new();
        float strategy_updates = 0;

        public Bot CreateBot(Position position)
        {
            Bot bot = Create(position);
            bots.Add(bot.Id, bot);

            UpdateBotStrategy(bot);

            return bot;
        }

        public void RemoveBot(Guid id)
        {
            bots.Remove(id);
        }

        Bot Create(Position position)
        {
            return new(Guid.NewGuid(), "Bot", position, 3, 1, 2, 120f);
        }

        public void Update()
        {
            strategy_updates++;

            if (strategy_updates >= STRATEGY_UPDATE_FREQUENCY)
            {
                UpdateAllStrategies();
                strategy_updates = 0;
            }

            foreach (var bot in bots)
                bot.Value.Update();

        }

        void UpdateAllStrategies()
        {
            foreach (var bot in bots)
                UpdateBotStrategy(bot.Value);
        }

        void UpdateBotStrategy(Bot bot)
        {
            bot.SetStrategy(new TestBotStrategy());
        }
    }
}
