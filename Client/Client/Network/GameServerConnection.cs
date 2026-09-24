using Bomber.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Bomber.Client.Network;

public sealed class GameServerConnection
{
    private readonly HubConnection _connection;

    public event Action<Guid>? PlayerAssigned;

    public event Action<GameStateResponse>? GameStateUpdated;

    public GameServerConnection(string serverUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"{serverUrl}/game")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<Guid>("PlayerAssigned", OnPlayerAssigned);

        _connection.On<GameStateResponse>("GameStateUpdated", OnGameStateUpdated);
    }

    private void OnPlayerAssigned(Guid playerId)
    {
        PlayerAssigned?.Invoke(playerId);
    }

    private void OnGameStateUpdated(
        GameStateResponse gameState)
    {
        GameStateUpdated?.Invoke(gameState);
    }

    public Task ConnectAsync()
    {
        return _connection.StartAsync();
    }

    public Task DisconnectAsync()
    {
        return _connection.StopAsync();
    }

    public Task SendMovementAsync(
        MoveRequest request)
    {
        return _connection.InvokeAsync("Move", request);
    }
}