using Bomber.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Bomber.Client.Network;

public sealed class GameServerConnection
{
    private readonly HubConnection _connection;

    public event Action<Guid>? PlayerAssigned;

    public event Action<GameStateResponse>? GameStateUpdated;

    public event Action<BombExplodedResponse>? BombExploded;

    public GameServerConnection(string serverUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"{serverUrl}/game")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<Guid>("PlayerAssigned", OnPlayerAssigned);

        _connection.On<GameStateResponse>("GameStateUpdated", OnGameStateUpdated);

        _connection.On<BombExplodedResponse>("BombExploded", OnBombExploded);
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

    public Task SendPlaceBombAsync()
    {
        return _connection.InvokeAsync("PlaceBomb");
    }

    public Task SendUndoAsync()
    {
        return _connection.InvokeAsync("UndoPlaceBomb");
    }

    public Task<bool> TogglePiercingAsync()
    {
        return _connection.InvokeAsync<bool>("TogglePiercing");
    }

    public Task<bool> ToggleMegaBlastAsync()
    {
        return _connection.InvokeAsync<bool>("ToggleMegaBlast");
    }

    public Task<bool> ToggleRemoteDetonationAsync()
    {
        return _connection.InvokeAsync<bool>("ToggleRemoteDetonation");
    }

    public Task SendDetonateBombsAsync()
    {
        return _connection.InvokeAsync("DetonateBombs");
    }

    private void OnBombExploded(BombExplodedResponse explosion)
    {
        BombExploded?.Invoke(explosion);
    }
}