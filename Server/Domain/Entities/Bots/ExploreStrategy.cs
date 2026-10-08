using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using Bomber.Domain.PowerUps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Bomber.Domain.Entities.Bots
{
    public class ExploreStrategy : BotStrategy
    {
        private const float TILE_SIZE = 40f;
        private const float BOT_SIZE = 30f;

        GridPosition target;
        bool spawnedBomb = false;

        public ExploreStrategy()
        {
            target = new(-1, -1);
        }

        public ExploreStrategy(Bot bot)
        {
            target = new(-1, -1);
            SetBot(bot);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if(!spawnedBomb && target.X != -1 && target.Y != -1)
            {
                GridPosition botPos = GetGridPosition(CurrentBot.Position);

                float distance = (Math.Abs(target.X - botPos.X) + Math.Abs(target.Y - botPos.Y));

                if (distance < 0.5f)
                {
                    spawnedBomb = true;
                    Manager.OnBombRequest?.Invoke(CurrentBot.Id);
                    Manager.UpdateBotStrategy(CurrentBot);
                }
            }

        }

        protected override bool FindTarget(out GridPosition position)
        {
            if (target.X == -1 || target.Y == -1)
                target = FindTarget();

            position = new(target.X, target.Y);
            return target.X != -1 && target.Y != -1;
        }

        GridPosition FindTarget()
        {
            return GetClosestPosition(Manager.GameMap.GetAllWalkablePositions().Where(p => IsNearbyWall(p)).ToList());
        }

        GridPosition GetClosestPosition(List<GridPosition> positions)
        {
            GridPosition closest = new(-1, -1);
            float cDistance = float.MaxValue;

            GridPosition botPos = GetGridPosition(CurrentBot.Position);

            foreach (var p in positions)
            {
                float distance = (p.X - botPos.X) * (p.X - botPos.X) + (p.Y - botPos.Y) * (p.Y - botPos.Y);

                if (distance < cDistance)
                {
                    cDistance = distance;
                    closest = p;
                }
            }

            return closest;
        }

        bool IsNearbyWall(GridPosition position)
        {
            List<(int, int)> directions = new() { (0, 1), (1, 0), (-1, 0), (0, -1) };

            foreach((int dx, int dy) in directions)
            {
                GridPosition newPos = new(position.X + dx, position.Y + dy);
                if (Manager.GameMap.IsInsideMap(newPos) && Manager.GameMap.GetCell(newPos) is DestructibleWall)
                    return true;
            }

            return false;
        }

        public override float GetWeight()
        {
            GridPosition target = FindTarget();

            if (target.X == -1 || target.Y == -1)
                return 0f;

            GridPosition botPos = GetGridPosition(CurrentBot.Position);
            float distance = (Math.Abs(target.X - botPos.X) + Math.Abs(target.Y - botPos.Y));

            return 5 - Math.Min(distance, 5) * 0.35f;
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
