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

        bool _spawnedBomb = false;

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

            if (_spawnedBomb)
                return;

            Player target = Manager.GetClosestPlayer(CurrentBot);

            if (target.Id == Guid.Empty)
                return;

            float distance = (Math.Abs(target.Position.X - CurrentBot.Position.X) + Math.Abs(target.Position.Y - CurrentBot.Position.Y)) / TILE_SIZE;

            if (distance < 1f)
            {
                _spawnedBomb = true;
                Manager.OnBombRequest?.Invoke(CurrentBot.Id);
                Manager.UpdateBotStrategy(CurrentBot);
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
