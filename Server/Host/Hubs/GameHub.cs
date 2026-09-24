using Bomber.Application.Models;
using Bomber.Application.Services;
using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using Bomber.Host.Services;
using Microsoft.AspNetCore.SignalR;

namespace Bomber.Host.Hubs;

public sealed class GameHub : Hub
{
    private readonly GameStateService _gameState;
    private readonly ConnectionRegistry _connections;

    public GameHub(
        GameStateService gameState,
        ConnectionRegistry connections)
    {
        _gameState = gameState;
        _connections = connections;
    }

    public override async Task OnConnectedAsync()
    {
        Guid playerId = Guid.NewGuid();

        _connections.Add(Context.ConnectionId, playerId);

        string playerName = $"Player {playerId.ToString()[..4]}";

        _gameState.AddPlayer(playerId, playerName);

        await Clients.Caller.SendAsync("PlayerAssigned", playerId);

        await BroadcastGameState();

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        bool found = _connections.Remove(Context.ConnectionId, out Guid playerId);

        if (found)
        {
            _gameState.RemovePlayer(playerId);

            await BroadcastGameState();
        }

        await base.OnDisconnectedAsync(
            exception);
    }

    public async Task Move(MoveRequest request)
    {
        bool found =
            _connections.TryGetPlayerId(
                Context.ConnectionId,
                out Guid playerId);

        if (!found)
            return;

        _gameState.MovePlayer(
            playerId,
            request.Up,
            request.Down,
            request.Left,
            request.Right);

        await BroadcastGameState();
    }

    private async Task BroadcastGameState()
    {
        Map map = _gameState.GetMap();

        List<MapCellResponse> cells = CreateMapCellResponses(map);
        List<SpawnPointResponse> spawnPoints = CreateSpawnPointResponses(map);
        List<PlayerStateResponse> players = CreatePlayerResponses();

        GameStateResponse response =
            new(
                map.Width,
                map.Height,
                cells,
                spawnPoints,
                players);

        await Clients.All.SendAsync("GameStateUpdated", response);
    }

    private static List<MapCellResponse> CreateMapCellResponses(Map map)
    {
        List<MapCellResponse> responses = [];

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                GridPosition position = new(x, y);

                MapCell cell = map.GetCell(position);

                MapCellType type = GetMapCellType(cell);

                MapCellResponse response =
                    new(
                        cell.Id,
                        cell.Position.X,
                        cell.Position.Y,
                        type);

                responses.Add(response);
            }
        }

        return responses;
    }

    private static List<SpawnPointResponse> CreateSpawnPointResponses(Map map)
    {
        List<SpawnPointResponse> responses = [];

        foreach (SpawnPoint spawnPoint in map.SpawnPoints)
        {
            SpawnPointResponse response =
                new(
                    spawnPoint.Id,
                    spawnPoint.Position.X,
                    spawnPoint.Position.Y,
                    spawnPoint.PlayerIndex);

            responses.Add(response);
        }

        return responses;
    }

    private List<PlayerStateResponse> CreatePlayerResponses()
    {
        List<Player> players = _gameState.GetPlayers();

        List<PlayerStateResponse> responses = [];

        foreach (Player player in players)
        {
            bool isAlive = player.State == PlayerState.Alive;

            PlayerStateResponse response =
                new(
                    player.Id,
                    player.Name,
                    player.Position.X,
                    player.Position.Y,
                    player.Lives,
                    player.BombLimit,
                    player.ExplosionRadius,
                    player.Speed,
                    isAlive);

            responses.Add(response);
        }

        return responses;
    }

    private static MapCellType GetMapCellType(MapCell cell)
    {
        if (cell is DestructibleWall)
            return MapCellType.DestructibleWall;

        if (cell is IndestructibleWall)
            return MapCellType.IndestructibleWall;

        return MapCellType.Floor;
    }
}