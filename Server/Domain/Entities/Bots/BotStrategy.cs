using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Entities.Bots
{
    public class BotStrategy
    {
        protected BotManager Manager { private set; get; }

        public BotStrategy(BotManager botManager)
        {
            Manager = botManager;
        }

        public virtual void Update(Bot bot, float deltaTime)
        {

        }
    }
}