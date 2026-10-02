using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Entities.Bots
{
    public class BotStrategy
    {
        protected BotManager Manager => BotManager.Instance;

        public virtual void Update(Bot bot, float deltaTime)
        {

        }
    }
}