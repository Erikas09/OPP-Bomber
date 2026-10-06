using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Players;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Bomber.Domain.PowerUps;

namespace Bomber.Domain.Entities.Bots
{
    public class PowerUpStrategy : BotStrategy
    {
        private const float TILE_SIZE = 40f;

        public PowerUpStrategy()
        {

        }

        public PowerUpStrategy(Bot bot)
        {
            SetBot(bot);
        }

        protected override bool FindTarget(out GridPosition position)
        {
            PowerUp? target = GetClosestPowerup();

            if (target == null)
            {
                position = new(-1, -1);
                return false;
            }

            position = new(target.position_x, target.position_y);
            return true;
        }

        public override float GetWeight()
        {
            PowerUp? target = GetClosestPowerup();

            if (target == null)
                return 0f;

            Position position = GetWorldPosition(new(target.position_x, target.position_y));
            float distance = (Math.Abs(position.X - CurrentBot.Position.X) + Math.Abs(position.Y - CurrentBot.Position.Y)) / TILE_SIZE;

            return 5 - Math.Min(distance, 5);
        }

        PowerUp? GetClosestPowerup()
        {
            PowerUp? closest = null;
            float cDistance = float.MaxValue;

            var powerups = Manager.GameMap.GetPowerUps();

            foreach (var p in powerups)
            {
                Position position = GetWorldPosition(new(p.position_x, p.position_y));

                float distance = (position.X - CurrentBot.Position.X) * (position.X - CurrentBot.Position.X) +
                    (position.Y - CurrentBot.Position.Y) * (position.Y - CurrentBot.Position.Y);

                if (distance < cDistance)
                {
                    cDistance = distance;
                    closest = p;
                }
            }

            return closest;
        }

        Position GetWorldPosition(GridPosition position)
        {
            return new(
                (position.X + 0.5f) * TILE_SIZE,
                (position.Y + 0.5f) * TILE_SIZE
            );
        }
    }
}
