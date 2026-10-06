using Bomber.Domain.Common;
using Bomber.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Entities.Bots
{
    public class BotStrategy
    {
        protected BotManager Manager => BotManager.Instance;
        protected Bot CurrentBot { private set; get; }

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

        public BotStrategy()
        {
            CurrentBot = null!;
            currentGrid = new(-1, -1);
            targetGrid = new(-1, -1);
            nextGrid = new(-1, -1);
        }

        public void SetBot(Bot bot)
        {
            CurrentBot = bot;
        }

        public virtual void Update(float deltaTime)
        {
            if (!initialized)
            {
                currentGrid = GetGridPosition(CurrentBot.Position);
                initialized = true;
            }

            pathUpdateTimer -= deltaTime;

            GridPosition target;

            if(!FindTarget(out target))
            {
                Stop();
                return;
            }

            GridPosition newTargetGrid = target;

            bool targetMoved = newTargetGrid.X != targetGrid.X || newTargetGrid.Y != targetGrid.Y;

            bool needsPath = path.Count == 0 || targetMoved || pathUpdateTimer <= 0f;

            if (needsPath)
            {
                UpdatePath(newTargetGrid);
                pathUpdateTimer = PATH_UPDATE_TIME;
            }

            Move();
        }

        public virtual float GetWeight()
        {
            return 0f;
        }

        protected virtual bool FindTarget(out GridPosition position)
        {
            position = new(-1, -1);
            return false;
        }

        private void UpdatePath(GridPosition newTargetGrid)
        {
            GridPosition botGrid = GetGridPosition(CurrentBot.Position);
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

        private void Move()
        {
            if (nextGrid.X < 0 || nextGrid.Y < 0)
            {
                if (path.Count > 0)
                    nextGrid = path.Pop();

                Stop();
                return;
            }

            float botCenterX = CurrentBot.Position.X + BOT_SIZE * 0.5f;
            float botCenterY = CurrentBot.Position.Y + BOT_SIZE * 0.5f;

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

                Stop();
                return;
            }


            if (dirY < -TILE_REACH_DISTANCE)
                CurrentBot.SetInput(new(true, false, false, false));
            else if (dirY > TILE_REACH_DISTANCE)
                CurrentBot.SetInput(new(false, true, false, false));
            else if (dirX < -TILE_REACH_DISTANCE)
                CurrentBot.SetInput(new(false, false, true, false));
            else if (dirX > TILE_REACH_DISTANCE)
                CurrentBot.SetInput(new(false, false, false, true));
        }

        private void Stop()
        {
            CurrentBot.SetInput(new(false, false, false, false));
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