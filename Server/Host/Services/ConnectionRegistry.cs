namespace Bomber.Host.Services;

public sealed class ConnectionRegistry
{
    private readonly Lock _lock = new();

    private readonly Dictionary<string, Guid> _connections = [];

    public void Add(
        string connectionId,
        Guid playerId)
    {
        lock (_lock)
            _connections[connectionId] = playerId;
    }

    public bool TryGetPlayerId(
        string connectionId,
        out Guid playerId)
    {
        lock (_lock)
            return _connections.TryGetValue(connectionId, out playerId);
    }

    public bool Remove(
        string connectionId,
        out Guid playerId)
    {
        lock (_lock)
        {
            bool found = _connections.TryGetValue(connectionId, out playerId);

            if (!found)
                return false;

            _connections.Remove(connectionId);

            return true;
        }
    }
}
