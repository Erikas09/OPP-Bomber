using Bomber.Application.Models;
using Bomber.Application.Services;
using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using Bomber.Host.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Bomber.Host.Services;

public sealed class GameStateBroadcaster
{
    private readonly GameStateService _gameState;
    private readonly IHubContext<GameHub> _hub;

    public GameStateBroadcaster(
        GameStateService gameState,
        IHubContext<GameHub> hub)
    {
        _gameState = gameState;
        _hub = hub;
    }

    public async Task BroadcastAsync(
        CancellationToken cancellationToken = default)
    {
        Map map = _gameState.GetMap();

        List<MapCellResponse> cells =
            CreateMapCellResponses(map);

        List<SpawnPointResponse> spawnPoints =
            CreateSpawnPointResponses(map);

        List<PlayerStateResponse> players =
            CreatePlayerResponses();

        List<BombResponse> bombs =
            CreateBombResponses();

        GameStateResponse response =
            new(
                map.Width,
                map.Height,
                cells,
                spawnPoints,
                players,
                bombs);

        await _hub.Clients.All.SendAsync(
            "GameStateUpdated",
            response,
            cancellationToken);
    }

    private List<PlayerStateResponse> CreatePlayerResponses()
    {
        List<Player> players =
            _gameState.GetPlayers();

        List<PlayerStateResponse> responses = [];

        foreach (Player player in players)
        {
            responses.Add(
                new PlayerStateResponse(
                    player.Id,
                    player.Name,
                    player.Position.X,
                    player.Position.Y,
                    player.Lives,
                    player.BombLimit,
                    player.ExplosionRadius,
                    player.Speed,
                    player.State == PlayerState.Alive));
        }

        return responses;
    }

    private List<BombResponse> CreateBombResponses()
    {
        List<Bomb> bombs =
            _gameState.GetBombs();

        List<BombResponse> responses = [];

        foreach (Bomb bomb in bombs)
        {
            responses.Add(
                new BombResponse(
                    bomb.Id,
                    bomb.OwnerId,
                    bomb.Position.X,
                    bomb.Position.Y,
                    bomb.Radius));
        }

        return responses;
    }

    private static List<MapCellResponse> CreateMapCellResponses(
        Map map)
    {
        List<MapCellResponse> responses = [];

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                GridPosition position = new(x, y);

                MapCell cell =
                    map.GetCell(position);

                MapCellType type =
                    GetMapCellType(cell);

                responses.Add(
                    new MapCellResponse(
                        cell.Id,
                        cell.Position.X,
                        cell.Position.Y,
                        type));
            }
        }

        return responses;
    }

    private static List<SpawnPointResponse> CreateSpawnPointResponses(
        Map map)
    {
        List<SpawnPointResponse> responses = [];

        foreach (SpawnPoint spawnPoint in map.SpawnPoints)
        {
            responses.Add(
                new SpawnPointResponse(
                    spawnPoint.Id,
                    spawnPoint.Position.X,
                    spawnPoint.Position.Y,
                    spawnPoint.PlayerIndex));
        }

        return responses;
    }

    private static MapCellType GetMapCellType(MapCell cell)
    {
        if (cell is ClassicDestructibleWall)
            return MapCellType.DestructibleWall;

        if (cell is ClassicIndestructibleWall)
            return MapCellType.IndestructibleWall;

        if (cell is ClassicFloor)
            return MapCellType.Floor;

        if (cell is IceDestructibleWall)
            return MapCellType.IceDestructibleWall;

        if (cell is IceIndestructibleWall)
            return MapCellType.IceIndestructibleWall;

        return MapCellType.IceFloor;
    }
}
