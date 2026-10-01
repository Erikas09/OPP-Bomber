using Bomber.Application.Interfaces;
using Bomber.Domain.Events;

namespace Bomber.Application.Observers;

public sealed class PlayerDeathObserver : IGameObserver
{
    private readonly Lock _lock = new();

    private readonly Dictionary<Guid, int> _deaths = [];

    public void Update(GameEvent gameEvent)
    {
        if (gameEvent is not PlayerDiedEvent playerDied)
            return;

        lock (_lock)
        {
            if (!_deaths.ContainsKey(playerDied.PlayerId))
                _deaths[playerDied.PlayerId] = 0;

            _deaths[playerDied.PlayerId]++;
        }
    }

    public int GetDeathCount(Guid playerId)
    {
        lock (_lock)
        {
            if (!_deaths.TryGetValue(playerId, out int deaths))
                return 0;

            return deaths;
        }
    }
}
