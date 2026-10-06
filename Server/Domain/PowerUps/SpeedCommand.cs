using Bomber.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.PowerUps
{
    internal class SpeedCommand : IPowerUpCommand
    {
        SpeedPowerUp powerUp;

        public SpeedCommand(SpeedPowerUp powerUp)
        {
            this.powerUp = powerUp;
        }
        public void Execute(Player player)
        {
            powerUp.ApplyEffect(player);
        }

        public void Undo(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
