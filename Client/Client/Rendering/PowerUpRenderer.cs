using Bomber.Client.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Client.Rendering
{
    public sealed class PowerUpRenderer
    {
        private const int TileSize = 40;

        public void Draw(Graphics graphics, IReadOnlyCollection<PowerUpResponse> powerUps)
        {
            foreach (PowerUpResponse powerUp in powerUps)
            {
                int size = TileSize / 2;

                Rectangle rectangle = new(
                    powerUp.X * TileSize + (TileSize - size) / 2,
                    powerUp.Y * TileSize + (TileSize - size) / 2,
                    size,
                    size);

                Brush brush = GetBrush(powerUp.Type);

                graphics.FillRectangle(
                    brush,
                    rectangle);

                graphics.DrawRectangle(
                    Pens.Gray,
                    rectangle);
            }
        }

        private static Brush GetBrush(PowerUpType type)
        {
            return type switch
            {
                PowerUpType.Speed => Brushes.Aqua,
                PowerUpType.BlastRadius => Brushes.Chartreuse,
                PowerUpType.ExtraBomb => Brushes.Red,
                _ => Brushes.Magenta,
            };
        }

    }
}
