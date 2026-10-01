using Bomber.Domain.Enums;
using Bomber.Domain.PowerUps;

namespace Bomber.Application.Factories;

public sealed class PowerUpFactory
{
    public static PowerUp CreatePowerUp(PowerUpType type)
    {
        return type switch
        {
            PowerUpType.Speed => new SpeedPowerUp(30f),
            PowerUpType.BlastRadius => new BlastRadiusPowerUp(1f),
            PowerUpType.ExtraBomb => new ExtraBombPowerUp(1),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown power-up type."),
        };
    }
}
