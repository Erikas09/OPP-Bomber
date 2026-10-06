using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using Bomber.Domain.PowerUps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Bomber.Domain.Entities.Bots
{
    public class RunStrategy : BotStrategy
    {
        private const float TILE_SIZE = 40f;
        private const float BOT_SIZE = 30f;

        GridPosition target;

        public RunStrategy()
        {
            target = new(-1, -1);
        }

        public RunStrategy(Bot bot)
        {
            target = new(-1, -1);
            SetBot(bot);
        }

        protected override bool FindTarget(out GridPosition position)
        {
            if (target.X == -1 || target.Y == -1)
                target = GetRandomCell();

            position = new(target.X, target.Y);
            return target.X != -1 && target.Y != -1;
        }

        GridPosition GetRandomCell()
        {
            List<GridPosition> possibleCells = Manager.GameMap.GetAllWalkablePositions();
            var bombs = Manager.GetBombList();

            foreach (var b in bombs)
            {
                List<GridPosition> positions = ComputeExplosionCells(b.Position, b.Radius);
                possibleCells.RemoveAll(p => positions.Where(pp => p.X == pp.X && p.Y == pp.Y).Any());
            }

            if (possibleCells.Count == 0)
                return new(-1, -1);

            Random rng = new();
            return possibleCells[rng.Next(0, possibleCells.Count)];
        }

        public override float GetWeight()
        {
            var bombs = Manager.GetBombList();

            GridPosition position = GetGridPosition(CurrentBot.Position);

            foreach(var b in bombs)
            {
                if (IsInRadius(b, position))
                    return 25;
            }

            return 0;
        }

        bool IsInRadius(Bomb bomb, GridPosition botPosition)
        {
            List<GridPosition> positions = ComputeExplosionCells(bomb.Position, bomb.Radius);
            return positions.Where(p => p.X == botPosition.X && p.Y == botPosition.Y).Any();
        }

        private List<GridPosition> ComputeExplosionCells(GridPosition origin,int radius)
        {
            List<GridPosition> cells = [origin];

            (int dx, int dy)[] directions =
            [
                (0, -1),
                (0, 1),
                (-1, 0),
                (1, 0),
            ];

            foreach ((int dx, int dy) in directions)
            {
                for (int step = 1; step <= radius; step++)
                {
                    GridPosition current = new(
                        origin.X + dx * step,
                        origin.Y + dy * step);

                    if (!Manager.GameMap.IsInsideMap(current))
                        break;

                    MapCell cell = Manager.GameMap.GetCell(current);

                    if (cell is IndestructibleWall)
                        break;

                    cells.Add(current);

                    if (cell is DestructibleWall)
                        break;
                }
            }

            return cells;
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
