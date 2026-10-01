using Bomber.Application.Interfaces;
using Bomber.Domain.Events;

namespace Bomber.Application.Observers;

public sealed class WallDestroyedObserver : IGameObserver
{
    private readonly Lock _lock = new();

    private readonly Dictionary<Guid, int> _destroyedWalls = [];

    public void Update(GameEvent gameEvent)
    {
        if (gameEvent is not WallDestroyedEvent wallDestroyed)
            return;

        lock (_lock)
        {
            if (!_destroyedWalls.ContainsKey(wallDestroyed.CausedByPlayerId))
                _destroyedWalls[wallDestroyed.CausedByPlayerId] = 0;

            _destroyedWalls[wallDestroyed.CausedByPlayerId]++;
        }
    }

    public int GetDestroyedWallCount(Guid playerId)
    {
        lock (_lock)
        {
            if (!_destroyedWalls.TryGetValue(playerId, out int count))
                return 0;

            return count;
        }
    }
}
