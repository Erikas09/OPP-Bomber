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
        private const float TILE_REACH_DISTANCE = 1f;

        private const float PATH_UPDATE_TIME = 1f;

        float pathUpdateTimer;

        Stack<GridPosition> path = new();

        GridPosition currentGrid;
        GridPosition targetGrid;
        GridPosition nextGrid;

        bool initialized;

        public HuntStrategy( )
        {
            currentGrid = new(-1, -1);
            targetGrid = new(-1, -1);
            nextGrid = new(-1, -1);
        }

        public override void Update(Bot bot, float deltaTime)
        {
            if (!initialized)
            {
                currentGrid = GetGridPosition(bot.Position);
                initialized = true;
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

        private void UpdatePath(Bot bot, GridPosition newTargetGrid)
        {
            GridPosition botGrid = GetGridPosition(bot.Position);
            currentGrid = botGrid;

            targetGrid = newTargetGrid;

            Stack<GridPosition> newPath = Manager.FindPath(currentGrid, targetGrid);

            if (newPath.Count == 0)
            {
                path.Clear();
                return;
            }

            if (newPath.Peek().X == currentGrid.X && newPath.Peek().Y == currentGrid.Y)
                newPath.Pop();


            if (newPath.Count == 0)
            {
                path.Clear();
                return;
            }

            path = newPath;
        }

        private void Move(Bot bot)
        {
            if (nextGrid.X < 0 || nextGrid.Y < 0)
            {
                if (path.Count > 0)
                    nextGrid = path.Pop();

                Stop(bot);
                return;
            }

            float botCenterX = bot.Position.X + BOT_SIZE * 0.5f;
            float botCenterY = bot.Position.Y + BOT_SIZE * 0.5f;

            Position nextWorld = GetWorldPosition(nextGrid);

            float dirX = nextWorld.X - botCenterX;
            float dirY = nextWorld.Y - botCenterY;

            if (Math.Abs(dirX) <= TILE_REACH_DISTANCE && Math.Abs(dirY) <= TILE_REACH_DISTANCE)
            {
                currentGrid = nextGrid;

                if (path.Count > 0)
                {
                    nextGrid = path.Pop();
                }
                else
                {
                    nextGrid = new(-1, -1);
                }

                Stop(bot);
                return;
            }


            if (dirY < -TILE_REACH_DISTANCE)
                bot.SetInput(new(true, false, false, false));
            else if (dirY > TILE_REACH_DISTANCE)
                bot.SetInput(new(false, true, false, false));
            else if (dirX < -TILE_REACH_DISTANCE)
                bot.SetInput(new(false, false, true, false));
            else if (dirX > TILE_REACH_DISTANCE)
                bot.SetInput(new(false, false, false, true));
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

        Position GetWorldPosition(GridPosition position)
        {
            return new(
                (position.X + 0.5f) * TILE_SIZE,
                (position.Y + 0.5f) * TILE_SIZE
            );
        }
    }
}
