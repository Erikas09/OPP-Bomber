using Bomber.Application.Models;
using Bomber.Application.Services;
using Bomber.Application.Commands;
using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using Bomber.Domain.PowerUps;
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

        _connections.Add(Context.ConnectionId, playerId);

        string playerName = $"Player {playerId.ToString()[..4]}";

        _game.AddPlayer(playerId, playerName);

        await Clients.Caller.SendAsync("PlayerAssigned", playerId);

        await Broadcastgame();

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        bool found = _connections.Remove(Context.ConnectionId, out Guid playerId);

        if (found)
        {
            _game.RemovePlayer(playerId);

            await Broadcastgame();
        }

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

    private async Task Broadcastgame()
    {
        Map map = _game.GetMap();

        List<MapCellResponse> cells = CreateMapCellResponses(map);
        List<SpawnPointResponse> spawnPoints = CreateSpawnPointResponses(map);
        List<PlayerStateResponse> players = CreatePlayerResponses();
        List<BombResponse> bombs = CreateBombResponses();
        List<PowerUpResponse> powerUps = CreatePowerUpResponses(map);
        List<ExplosionResponse> explosions = CreateExplosionResponses();

        GameStateResponse response =
            new(
                map.Width,
                map.Height,
                cells,
                spawnPoints,
                players,
                bombs,
                powerUps,
                explosions);

        await Clients.All.SendAsync("gameUpdated", response);
    }

    private List<BombResponse> CreateBombResponses()
    {
        List<IBomb> bombs = _game.GetBombs();

        List<BombResponse> responses = [];

        foreach (IBomb bomb in bombs)
        {
            BombResponse response =
                new(
                    bomb.Id,
                    bomb.OwnerId,
                    bomb.Position.X,
                    bomb.Position.Y,
                    bomb.Radius);

            responses.Add(response);
        }

        return responses;
    }

    private List<ExplosionResponse> CreateExplosionResponses()
    {
        List<Explosion> explosions = _game.GetExplosions();

        List<ExplosionResponse> responses = [];

        foreach (Explosion explosion in explosions)
        {
            ExplosionResponse response =
                new(
                    explosion.Id,
                    explosion.BombId,
                    explosion.OwnerId,
                    [.. explosion.Cells]);

            responses.Add(response);
        }

        return responses;
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
        List<Player> players = _game.GetPlayers();

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

    private List<PowerUpResponse> CreatePowerUpResponses(Map map)
    {
        List<PowerUpResponse> responses = [];
        List<PowerUp> powerUps = map.GetPowerUps();

        foreach (PowerUp powerUp in powerUps)
        {
            PowerUpType type = GetPowerUpType(powerUp);

            PowerUpResponse response =
                new(
                    powerUp.Id,
                    powerUp.position_x,
                    powerUp.position_y,
                    type);

            responses.Add(response);
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

    private static PowerUpType GetPowerUpType(PowerUp powerUp)
    {
        if (powerUp is BlastRadiusPowerUp)
            return PowerUpType.BlastRadius;

        if (powerUp is SpeedPowerUp)
            return PowerUpType.Speed;

        return PowerUpType.ExtraBomb;
    }

    public async Task PlaceBomb()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);

        if (!found)
            return;

        PlaceBombCommand command = new(_game, playerId);
        bool placed = command.Execute();

        if(placed)
            _game.AddBombCommand(command);
    }

    public async Task UndoPlaceBomb()
    {
        bool found =_connections.TryGetPlayerId(Context.ConnectionId,out Guid playerId);

        if (!found)
            return;

        List<PlaceBombCommand> commands = _game.GetBombCommands();

        PlaceBombCommand latestCommand = commands.LastOrDefault(a => a.GetPlayerId() == playerId);

        if (latestCommand is null)
            return;

        bool success = latestCommand.Undo();

        if (success)
            commands.Remove(latestCommand);
    }

    public Task<bool> TogglePiercing()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);
        if (!found)
            return Task.FromResult(false);

        return Task.FromResult(_game.TogglePlayerPiercing(playerId));
    }

    public Task<bool> ToggleMegaBlast()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);
        if (!found)
            return Task.FromResult(false);

        return Task.FromResult(_game.TogglePlayerMegaBlast(playerId));
    }

    public Task<bool> ToggleRemoteDetonation()
    {
        bool found = _connections.TryGetPlayerId(Context.ConnectionId, out Guid playerId);
        if (!found)
            return Task.FromResult(false);

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