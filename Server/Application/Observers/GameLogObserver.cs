using Bomber.Application.Interfaces;
using Bomber.Domain.Events;

namespace Bomber.Application.Observers;

public sealed class GameLogObserver : IGameObserver
{
    public void Update(
        GameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case BombExplodedEvent bombExploded:
                Console.WriteLine(
                    $"Bomb {bombExploded.BombId} exploded " +
                    $"at ({bombExploded.Origin.X}, " +
                    $"{bombExploded.Origin.Y}).");
                break;

            case WallDestroyedEvent wallDestroyed:
                Console.WriteLine(
                    $"Wall {wallDestroyed.WallId} was destroyed " +
                    $"by player {wallDestroyed.CausedByPlayerId}.");
                break;

            case PlayerDamagedEvent playerDamaged:
                Console.WriteLine(
                    $"Player {playerDamaged.PlayerId} took damage. " +
                    $"Lives remaining: {playerDamaged.LivesRemaining}.");
                break;

            case PlayerDiedEvent playerDied:
                Console.WriteLine(
                    $"Player {playerDied.PlayerId} died.");
                break;
        }
    }
}
