using Bomber.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.PowerUps
{
    internal interface IPowerUpCommand
    {
        public abstract void Execute(Player player);
        public abstract void Undo(Player player);
    }
}
