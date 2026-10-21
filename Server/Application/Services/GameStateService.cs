using Bomber.Application.Commands;
using Bomber.Application.Publishers;
using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;
using Bomber.Domain.Enums;
using Bomber.Domain.Events;
using Bomber.Domain.Factories;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using Bomber.Domain.PowerUps;

namespace Bomber.Application.Services;

public sealed class GameStateService
{
    private readonly Lock _lock = new();

    private readonly List<PlaceBombCommand> _commands = [];

    private readonly PlayerSystem _players;
    private readonly BombSystem _bombs;
    private readonly Map _map;
    private readonly GameEventPublisher _eventPublisher;

    public GameStateService(GameEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;

        _map = Map.CreateTestMap(Map.MapType.Classic);
        _players = new PlayerSystem(_map);
        _bombs = new(_map, _players, _eventPublisher);

        BotManager.Initialize(_map, _players.AllPlayers);
        BotManager.Instance.OnBombRequest += id => PlaceBomb(id, out _);
        BotManager.Instance.GetBombList = GetBombs;

        SetGame();
    }

    public void SetGame()
    {
        _players.SpawnBots(_map.GetNewSpawnPoints(_players.PlayerCount, 2));
    }

    public void AddPlayer(Guid playerId, string playerName)
    {
        SpawnPoint? spawnpoint = _map.GetSpawnPoint(_players.PlayerCount);

        if (spawnpoint == null)
            return;

        _players.AddPlayer(playerId, playerName, spawnpoint);
    }

    public void AddPlayer(Player player)
    {
        _players.AddPlayer(player);
    }

    public void RemovePlayer(Guid playerId)
    {
        _players.RemovePlayer(playerId);
    }

    public List<Player> GetPlayers()
    {
        return _players.GetPlayers();
    }

    public void SetPlayerInput(
        Guid playerId,
        PlayerInput input)
    {
        lock (_lock)
        {
            if (!_players.AllPlayers.ContainsKey(playerId))
                return;

            _players.AllPlayers[playerId].SetInput(input);
        }
    }

    public bool PlaceBomb(Guid playerId, out Bomb? bomb)
    {
        return _bombs.PlaceBomb(_players.AllPlayers[playerId], out bomb);
    }

    public bool RemoveBomb(Guid bombId)
    {
        return _bombs.RemoveBomb(bombId);
    }

    public void Update(float deltaTime)
    {
        lock (_lock)
        {
            _players.Update(deltaTime);
            BotManager.Instance.Update(deltaTime);
            _bombs.Update(deltaTime);
        }
    }

    public Map GetMap()
    {
        return _map;
    }

    public List<PlaceBombCommand> GetBombCommands()
    {
        return _commands;
    }

    public void AddBombCommand(PlaceBombCommand command)
    {
        _commands.Add(command);
    }

    public List<Bomb> GetBombsToExplode(DateTimeOffset now)
    {
        return _bombs.GetBombsToExplode(now);
    }

    public List<Bomb> GetBombs()
    {
        return _bombs.GetBombs();
    }

    public List<Explosion> GetExplosions()
    {
        return _bombs.GetExplosions();
    }
}