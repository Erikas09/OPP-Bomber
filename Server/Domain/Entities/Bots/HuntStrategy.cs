using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Players;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Bomber.Domain.Entities.Bots
{
    public class HuntStrategy : BotStrategy
    {
        private const float TILE_SIZE = 40f;
        private const float BOT_SIZE = 30f;

        const float BOMB_SPAWN_DELAY = 1.5f;

        float _bombTimer = 0;

        public HuntStrategy()
        {

        }

        public HuntStrategy(Bot bot)
        {
            SetBot(bot);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            Player target = Manager.GetClosestPlayer(CurrentBot);

            if (target.Id == Guid.Empty)
                return;

            float distance = (Math.Abs(target.Position.X - CurrentBot.Position.X) + Math.Abs(target.Position.Y - CurrentBot.Position.Y)) / TILE_SIZE;

            _bombTimer += deltaTime;

            if(_bombTimer >= BOMB_SPAWN_DELAY)
            {
                if (distance < 1f)
                {
                    Manager.OnBombRequest?.Invoke(CurrentBot.Id);
                    _bombTimer = 0;
                }
            }
        }

        protected override bool FindTarget(out GridPosition position)
        {
            Player target = Manager.GetClosestPlayer(CurrentBot);

            if (target.Id == Guid.Empty)
            {
                position = new(-1, -1);
                return false;
            }

            position = GetGridPosition(target.Position);
            return true;
        }

        public override float GetWeight()
        {
            Player target = Manager.GetClosestPlayer(CurrentBot);

            if (target.Id == Guid.Empty)
                return 0f;

            float distance = (Math.Abs(target.Position.X - CurrentBot.Position.X) + Math.Abs(target.Position.Y - CurrentBot.Position.Y)) / TILE_SIZE;

            return 5 - Math.Min(distance, 5);
        }

        GridPosition GetGridPosition(Position pos)
        {
            float centerX = pos.X + BOT_SIZE / 2f;
            float centerY = pos.Y + BOT_SIZE / 2f;

            int gridX = (int)(centerX / TILE_SIZE);
            int gridY = (int)(centerY / TILE_SIZE);

            return new GridPosition(gridX, gridY);
        }
    }
}
