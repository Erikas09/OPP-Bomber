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

public sealed class GameFacade
{
    private readonly Lock _lock = new();
    private readonly PlayerSystem _players;
    private readonly BombSystem _bombs;
    private readonly Map _map;

    public GameFacade(Map map, PlayerSystem players, BombSystem bombs)
    {
        _map = map;
        _players = players;
        _bombs = bombs;
    }

    public void SetGame()
    {
        _players.SpawnBots(_map.GetNewSpawnPoints(_players.PlayerCount, 2));
    }

    public void AddPlayer(Guid playerId, string playerName)
    {
        lock (_lock)
        {
            SpawnPoint? spawnpoint = _map.GetSpawnPoint(_players.PlayerCount);

            if (spawnpoint == null)
                return;

            _players.AddPlayer(playerId, playerName, spawnpoint);
        }
    }

    public void AddPlayer(Player player)
    {
        lock (_lock)
        {
            _players.AddPlayer(player);
        }
    }

    public void RemovePlayer(Guid playerId)
    {
        lock (_lock)
        {
            _players.RemovePlayer(playerId);
        }
    }

    public List<Player> GetPlayers()
    {
        return _players.GetPlayers();
    }

    public void SetPlayerInput(Guid playerId, PlayerInput input)
    {
        lock (_lock)
        {
            _players.SetPlayerInput(playerId, input);
        }
    }

    public bool PlaceBomb(Guid playerId, out IBomb? bomb)
    {
        lock (_lock)
        {
            if (!_players.AllPlayers.TryGetValue(playerId, out var player))
            {
                bomb = null;
                return false;
            }

            return _bombs.PlaceBomb(player, out bomb);
        }
    }

    public void DetonateBombs(Guid playerId)
    {
        _bombs.DetonatePlayerBombs(playerId);
    }

    public bool TogglePlayerPiercing(Guid playerId)
    {
        if (_players.AllPlayers.TryGetValue(playerId, out Player? player))
            return player.TogglePiercingBombs();
        return false;
    }

    public bool TogglePlayerMegaBlast(Guid playerId)
    {
        if (_players.AllPlayers.TryGetValue(playerId, out Player? player))
            return player.ToggleMegaBlastBombs();
        return false;
    }

    public bool TogglePlayerRemoteDetonation(Guid playerId)
    {
        if (_players.AllPlayers.TryGetValue(playerId, out Player? player))
            return player.ToggleRemoteDetonationBombs();
        return false;
    }

    public bool RemoveBomb(Guid bombId)
    {
        lock (_lock)
        {
            return _bombs.RemoveBomb(bombId);
        }
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
        return _bombs.GetBombCommands();
    }

    public void AddBombCommand(PlaceBombCommand command)
    {
        lock (_lock)
        {
            _bombs.AddBombCommand(command);
        }
    }

    public List<IBomb> GetBombsToExplode(DateTimeOffset now)
    {
        return _bombs.GetBombsToExplode(now);
    }

    public List<IBomb> GetBombs()
    {
        return _bombs.GetBombs();
    }

    public List<Explosion> GetExplosions()
    {
        return _bombs.GetExplosions();
    }
}