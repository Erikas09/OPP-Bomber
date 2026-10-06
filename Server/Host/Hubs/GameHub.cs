using Bomber.Application.Models;
using Bomber.Application.Services;
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

        _gameState.SetPlayerInput(
            playerId,
            input);

        return Task.CompletedTask;
    }

    private async Task BroadcastGameState()
    {
        Map map = _gameState.GetMap();

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

        await Clients.All.SendAsync("GameStateUpdated", response);
    }

    private List<BombResponse> CreateBombResponses()
    {
        List<Bomb> bombs = _gameState.GetBombs();

        List<BombResponse> responses = [];

        foreach (Bomb bomb in bombs)
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
        List<Explosion> explosions = _gameState.GetExplosions();

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
        bool found =
            _connections.TryGetPlayerId(
                Context.ConnectionId,
                out Guid playerId);

        if (!found)
            return;

        bool placed = _gameState.PlaceBomb(playerId, out Bomb? bomb);

        if (!placed || bomb is null)
            return;

        await BroadcastGameState();

        _ = ScheduleExplosion(bomb.Id, bomb.FuseSeconds);
    }

    private async Task ScheduleExplosion(
        Guid bombId,
        int fuseSeconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(fuseSeconds));

        _gameState.ExplodeBomb(bombId);

        await BroadcastGameState();
    }
}