using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.PowerUps;

namespace Bomber.Domain.Factories;

public sealed class PowerUpFactory
{
    public static PowerUp CreatePowerUp(PowerUpType type, GridPosition position)
    {
        return type switch
        {
            PowerUpType.Speed => new SpeedPowerUp(1.25f, position),
            PowerUpType.BlastRadius => new BlastRadiusPowerUp(1, position),
            PowerUpType.ExtraBomb => new ExtraBombPowerUp(1, position),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown power-up type."),
        };
    }
}