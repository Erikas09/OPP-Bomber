using Bomber.Application.Models;
using Bomber.Application.Services;
using Bomber.Application.Commands;
using Bomber.Domain.Players;
using Bomber.Host.Services;
using Microsoft.AspNetCore.SignalR;

namespace Bomber.Host.Hubs;

public sealed class GameHub : Hub
{
    private readonly GameFacade _game;
    private readonly ConnectionRegistry _connections;

    public GameHub(
        GameFacade game,
        ConnectionRegistry connections)
    {
        _game = game;
        _connections = connections;

        //_game.OnBombRequest += id => _ = PlaceBotBomb(id);
    }

    public override async Task OnConnectedAsync()
    {
        Guid playerId = Guid.NewGuid();

        string playerName = $"Player {playerId.ToString()[..4]}";

        if (!_game.AddPlayer(playerId, playerName))
        {
            Context.Abort();
            throw new HubException("No spawn point is available.");
        }

        _connections.Add(Context.ConnectionId, playerId);

        await Clients.Caller.SendAsync("PlayerAssigned", playerId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        bool found = _connections.Remove(Context.ConnectionId, out Guid playerId);

        if (found)
            _game.RemovePlayer(playerId);

        await base.OnDisconnectedAsync(
            exception);
    }

    public Task Move(MoveRequest request)
    {
        bool found =
            _connections.TryGetPlayerId(
                Context.ConnectionId,
                out Guid playerId);

        if (!found)
            return Task.CompletedTask;

        PlayerInput input = new(
            request.Up,
            request.Down,
            request.Left,
            request.Right);

        _game.SetPlayerInput(
            playerId,
            input);

        return Task.CompletedTask;
    }

    public Task PlaceBomb()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);

        if (!found)
            return Task.CompletedTask;

        lock (_game.SyncRoot)
        {
            if (_game.IsPaused)
                return Task.CompletedTask;

            PlaceBombCommand command = new(_game, playerId);
            bool placed = command.Execute();

            if (placed)
                _game.AddBombCommand(command);
        }

        return Task.CompletedTask;
    }

    public Task UndoPlaceBomb()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);

        if (!found)
            return Task.CompletedTask;

        lock (_game.SyncRoot)
        {
            if (_game.IsPaused)
                return Task.CompletedTask;

            List<PlaceBombCommand> commands = _game.GetBombCommands();

            PlaceBombCommand? latestCommand = commands.LastOrDefault(a => a.GetPlayerId() == playerId);

            if (latestCommand is null)
                return Task.CompletedTask;

            bool success = latestCommand.Undo();

            if (success)
                commands.Remove(latestCommand);
        }

        return Task.CompletedTask;
    }

    public Task<bool> TogglePiercing()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);
        if (!found)
            return Task.FromResult(false);

        lock (_game.SyncRoot)
            return Task.FromResult(_game.TogglePlayerPiercing(playerId));
    }

    public Task<bool> ToggleMegaBlast()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);
        if (!found)
            return Task.FromResult(false);

        lock (_game.SyncRoot)
            return Task.FromResult(_game.TogglePlayerMegaBlast(playerId));
    }

    public Task<bool> ToggleRemoteDetonation()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);
        if (!found)
            return Task.FromResult(false);

        lock (_game.SyncRoot)
            return Task.FromResult(_game.TogglePlayerRemoteDetonation(playerId));
    }

    public Task DetonateBombs()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);
        if (found)
        {
            _game.DetonateBombs(playerId);
        }

        return Task.CompletedTask;
    }
}