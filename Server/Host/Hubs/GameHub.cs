using Bomber.Application.Models;
using Bomber.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Bomber.Host.Hubs;

public sealed class GameHub : Hub
{
    private readonly GameStateService _gameState;

    public GameHub(GameStateService gameState)
    {
        _gameState = gameState;
    }

    public override async Task OnConnectedAsync()
    {
        _gameState.AddPlayer(Context.ConnectionId);

        await Clients.Caller.SendAsync(
            "PlayerAssigned",
            Context.ConnectionId);

        await BroadcastGameState();

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        _gameState.RemovePlayer(Context.ConnectionId);

        await BroadcastGameState();

        await base.OnDisconnectedAsync(exception);
    }

    public async Task Move(MoveRequest request)
    {
        _gameState.MovePlayer(
            Context.ConnectionId,
            request.Up,
            request.Down,
            request.Left,
            request.Right);

        await BroadcastGameState();
    }

    private async Task BroadcastGameState()
    {
        var players = _gameState
            .GetPlayers()
            .Select(player =>
                new PlayerStateResponse {
                    Id = player.Id,
                    X = player.X,
                    Y = player.Y 
                })
            .ToList();

        await Clients.All.SendAsync(
            "GameStateUpdated",
            new GameStateResponse 
            { 
                Players = players
            });
    }
}