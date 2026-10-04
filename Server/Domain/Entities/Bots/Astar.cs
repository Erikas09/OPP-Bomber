using Bomber.Domain.Common;
using Bomber.Domain.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Domain.Entities.Bots
{
    public class Astar
    {
        class Cell
        {
            public GridPosition parent;
            public bool walkable;
            public float f;
            public float g;
            public float h;

            public Cell(bool walkable)
            {
                parent = new(-1, -1);
                this.walkable = walkable;
                f = float.MaxValue;
                g = float.MaxValue;
                h = float.MaxValue;
            }

            public Cell(GridPosition parent, float f)
            {
                this.parent = parent;
                this.f = f;
            }
        }

        Map map;

        public Astar(Map map)
        {
            this.map = map;
        }

        public Stack<GridPosition> FindPath(GridPosition startPosition, GridPosition target)
        {
            bool[,] closedList = new bool[map.Width, map.Height];
            Cell[,] cellDetails = GenerateDetails(map.Width, map.Height);

            int posX = startPosition.X;
            int posY = startPosition.Y;

            cellDetails[posX, posY].f = 0;
            cellDetails[posX, posY].g = 0;
            cellDetails[posX, posY].h = 0;
            cellDetails[posX, posY].parent = new(posX, posY);

            PriorityQueue<GridPosition, float> openList = new();
            openList.Enqueue(startPosition, 0);

            while (openList.Count > 0)
            {
                GridPosition pos = openList.Dequeue();

                if (closedList[pos.X, pos.Y])
                    continue;

                if (pos.X == target.X && pos.Y == target.Y)
                {
                    return GetPath(cellDetails, target);
                }

                closedList[pos.X, pos.Y] = true;

                GridPosition[] directions =
                {
                    new(1, 0),
                    new(0, 1),
                    new(-1, 0),
                    new(0, -1)
                };

                foreach (var dir in directions)
                {
                    int newPosX = pos.X + dir.X;
                    int newPosY = pos.Y + dir.Y;

                    if (newPosX < 0 || newPosX >= map.Width ||
                        newPosY < 0 || newPosY >= map.Height)
                    {
                        continue;
                    }

                    if (!cellDetails[newPosX, newPosY].walkable)
                        continue;

                    if (closedList[newPosX, newPosY])
                        continue;

                    float gNew = cellDetails[pos.X, pos.Y].g + 1f;
                    float hNew = CalculateHValue(newPosX, newPosY, target);
                    float fNew = gNew + hNew;

                    if (cellDetails[newPosX, newPosY].f == float.MaxValue ||
                        cellDetails[newPosX, newPosY].f > fNew)
                    {
                        cellDetails[newPosX, newPosY].g = gNew;
                        cellDetails[newPosX, newPosY].h = hNew;
                        cellDetails[newPosX, newPosY].f = fNew;
                        cellDetails[newPosX, newPosY].parent = pos;

                        openList.Enqueue(
                            new GridPosition(newPosX, newPosY),
                            fNew);
                    }
                }
            }

            return new();
        }

        Stack<GridPosition> GetPath(Cell[,] details, GridPosition target)
        {
            Stack<GridPosition> path = new();

            int x = target.X;
            int y = target.Y;

            while (details[x, y].parent.X != x || details[x, y].parent.Y != y)
            {
                path.Push(new(x, y));
                x = details[x, y].parent.X;
                y = details[x, y].parent.Y;
            }

            path.Push(new(x, y));

            return path;
        }

        Cell[,] GenerateDetails(int width, int height)
        {
            Cell[,] nodes = new Cell[width, height];

            for(int x = 0; x < width; x++)
                for(int y = 0; y < height; y++)
                {
                    nodes[x, y] = new(map.IsWalkable(new(x, y)));
                }

            return nodes;
        }

        static float CalculateHValue(int x, int y, GridPosition target)
        {
            return Math.Abs(x - target.X) + Math.Abs(y - target.Y);
        }
    }
}
