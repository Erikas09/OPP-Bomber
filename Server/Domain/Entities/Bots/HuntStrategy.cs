using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Players;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Bomber.Domain.Entities.Bots
{
    public class HuntStrategy : BotStrategy
    {
        private const float TILE_SIZE = 40f;
        private const float BOT_SIZE = 30f;
        private const float TILE_REACH_DISTANCE = 0.001f;

        private const float PATH_UPDATE_TIME = 1f;

        float pathUpdateTimer;

        Stack<GridPosition> path = new();

        GridPosition currentGrid;
        GridPosition targetGrid;
        GridPosition nextGrid;

        bool initialized;

        public HuntStrategy(BotManager manager) : base(manager)
        {
            currentGrid = new(-1, -1);
            targetGrid = new(-1, -1);
            nextGrid = new(-1, -1);
        }

        public override void Update(Bot bot, float deltaTime)
        {
            if (!initialized)
            {
                Initialize(bot);
            }

            pathUpdateTimer -= deltaTime;

            Player target = Manager.GetClosestPlayer(bot);

            if (target.Id == Guid.Empty)
            {
                Stop(bot);
                return;
            }

            GridPosition newTargetGrid = GetGridPosition(target.Position);

            bool targetMoved = newTargetGrid.X != targetGrid.X || newTargetGrid.Y != targetGrid.Y;

            bool needsPath = path.Count == 0 || targetMoved || pathUpdateTimer <= 0f;

            if (needsPath)
            {
                UpdatePath(bot, newTargetGrid);
                pathUpdateTimer = PATH_UPDATE_TIME;
            }

            Move(bot);
        }

        private void Initialize(Bot bot)
        {
            currentGrid = GetGridPosition(bot.Position);

            targetGrid = new(-1, -1);
            nextGrid = new(-1, -1);

            path.Clear();

            initialized = true;
        }

        private void UpdatePath(Bot bot, GridPosition newTargetGrid)
        {
            GridPosition botGrid = GetGridPosition(bot.Position);
            currentGrid = botGrid;

            targetGrid = newTargetGrid;

            Stack<GridPosition> newPath = Manager.FindPath(currentGrid, targetGrid);

            if (newPath.Count == 0)
            {
                path.Clear();
                nextGrid = new(-1, -1);
                return;
            }

            if (newPath.Peek().X == currentGrid.X && newPath.Peek().Y == currentGrid.Y)
                newPath.Pop();


            if (newPath.Count == 0)
            {
                path.Clear();
                nextGrid = new(-1, -1);
                return;
            }

            if (nextGrid.X >= 0 && IsOppositeDirection(currentGrid, nextGrid, newPath.Peek()))
            {
                if (path.Count > 0)
                {
                    return;
                }
            }

            path = newPath;
            nextGrid = path.Peek();
        }

        private void Move(Bot bot)
        {
            if (path.Count == 0)
            {
                Stop(bot);
                return;
            }

            GridPosition next = path.Peek();

            float dirX = next.X - currentGrid.X;
            float dirY = next.Y - currentGrid.Y;

            if (dirX * dirX + dirY * dirY <= TILE_REACH_DISTANCE * TILE_REACH_DISTANCE)
            {
                path.Pop();

                currentGrid = next;

                if (path.Count > 0)
                {
                    nextGrid = path.Peek();
                }
                else
                {
                    nextGrid = new(-1, -1);
                }

                Stop(bot);
                return;
            }

            if (dirY < 0)
                bot.SetInput(new(true, false, false, false));
            else if (dirY > 0)
                bot.SetInput(new(false, true, false, false));
            else if (dirX < 0)
                bot.SetInput(new(true, false, true, false));
            else if (dirX > 0)
                bot.SetInput(new(true, false, false, true));
        }

        private bool IsOppositeDirection(GridPosition current, GridPosition previousNext, GridPosition newNext)
        {
            int oldX = previousNext.X - current.X;
            int oldY = previousNext.Y - current.Y;

            int newX = newNext.X - current.X;
            int newY = newNext.Y - current.Y;

            return oldX + newX == 0 &&
                   oldY + newY == 0;
        }

        private void Stop(Bot bot)
        {
            bot.SetInput(new(false, false, false, false));
        }

        private GridPosition GetGridPosition(Position pos)
        {
            float centerX = pos.X + BOT_SIZE / 2f;
            float centerY = pos.Y + BOT_SIZE / 2f;

            int gridX = (int)(centerX / TILE_SIZE);
            int gridY = (int)(centerY / TILE_SIZE);

            return new GridPosition(gridX, gridY);
        }
    }
}
