using Bomber.Client.Interfaces;
using Bomber.Client.Models;

namespace Bomber.Client.Network;

public sealed class GameServerConnectionProxy : IGameServerConnection
{
    private const int RefreshIntervalMilliseconds = 500;

    private readonly IGameServerConnection _connection;

    private MoveRequest? _lastSentRequest;
    private long _lastSentTick;
    private long _forwardedRequests;
    private long _skippedRequests;

    public long ForwardedRequests => Interlocked.Read(ref _forwardedRequests);
    public long SkippedRequests => Interlocked.Read(ref _skippedRequests);

    public event Action<Guid>? PlayerAssigned
    {
        add => _connection.PlayerAssigned += value;
        remove => _connection.PlayerAssigned -= value;
    }

    public event Action<GameStateResponse>? GameStateUpdated
    {
        add => _connection.GameStateUpdated += value;
        remove => _connection.GameStateUpdated -= value;
    }

    public event Action<BombExplodedResponse>? BombExploded
    {
        add => _connection.BombExploded += value;
        remove => _connection.BombExploded -= value;
    }

    public GameServerConnectionProxy(IGameServerConnection connection)
    {
        _connection = connection;
    }

    public void ResetMovementCache()
    {
        _lastSentRequest = null;
        _lastSentTick = 0;
    }

    public Task ConnectAsync()
    {
        ResetMovementCache();

        return _connection.ConnectAsync();
    }

    public Task DisconnectAsync()
    {
        ResetMovementCache();

        return _connection.DisconnectAsync();
    }

    public async Task SendMovementAsync(MoveRequest request)
    {
        long now = Environment.TickCount64;

        if (_lastSentRequest == request &&
            now - _lastSentTick < RefreshIntervalMilliseconds)
        {
            Interlocked.Increment(ref _skippedRequests);
            return;
        }

        await _connection.SendMovementAsync(request);

        _lastSentRequest = request;
        _lastSentTick = Environment.TickCount64;
        Interlocked.Increment(ref _forwardedRequests);
    }

    public Task SendPlaceBombAsync()
    {
        return _connection.SendPlaceBombAsync();
    }

    public Task SendUndoAsync()
    {
        return _connection.SendUndoAsync();
    }

    public Task<bool> TogglePiercingAsync()
    {
        return _connection.TogglePiercingAsync();
    }

    public Task<bool> ToggleMegaBlastAsync()
    {
        return _connection.ToggleMegaBlastAsync();
    }

    public Task<bool> ToggleRemoteDetonationAsync()
    {
        return _connection.ToggleRemoteDetonationAsync();
    }

    public Task SendDetonateBombsAsync()
    {
        return _connection.SendDetonateBombsAsync();
    }
}
