using Bomber.Application.Commands;
using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Entities.Bots;
using Bomber.Domain.Maps;
using Bomber.Domain.Players;

namespace Bomber.Application.Services;

public sealed class GameFacade
{
    private readonly Lock _lock = new();
    private readonly PlayerSystem _players;
    private readonly BombSystem _bombs;
    private readonly Map _map;
    private readonly Dictionary<Guid, SpawnPoint> _assignedSpawnPoints = [];

    public Lock SyncRoot => _lock;
    public bool IsPaused { get; private set; }

    public GameFacade(Map map, PlayerSystem players, BombSystem bombs)
    {
        _map = map;
        _players = players;
        _bombs = bombs;
        _map.CaptureInitialLayout();
    }

    public void SetGame()
    {
        AddBots(BotType.Hunter, 1);
        AddBots(BotType.Defender, 1);
    }

    public bool AddPlayer(Guid playerId, string playerName)
    {
        lock (_lock)
        {
            if (_players.AllPlayers.ContainsKey(playerId))
                return false;

            SpawnPoint? spawnpoint = GetAvailableSpawnPoints().FirstOrDefault();

            if (spawnpoint == null)
                return false;

            _players.AddPlayer(playerId, playerName, spawnpoint);
            _assignedSpawnPoints.Add(playerId, spawnpoint);

            if (_players.GetPlayers().Count(p => p.State == PlayerState.Alive) >= 2)
                _players.ResetWinnerState();

            return true;
        }
    }

    public void AddPlayer(Player player)
    {
        lock (_lock)
        {
            if (_players.AllPlayers.ContainsKey(player.Id))
                return;

            SpawnPoint? spawnpoint = GetAvailableSpawnPoints().FirstOrDefault();

            if (spawnpoint is null)
                throw new InvalidOperationException("No spawn point is available.");

            player.MoveTo(Map.GridToWorldPosition(spawnpoint.Position, Player.PlayerSize));
            _players.AddPlayer(player);
            _assignedSpawnPoints.Add(player.Id, spawnpoint);
        }
    }

    public void RemovePlayer(Guid playerId)
    {
        lock (_lock)
        {
            if (!_players.AllPlayers.TryGetValue(playerId, out Player? player))
                return;

            if (player is Bot)
                BotManager.Instance.RemoveBot(playerId);

            _bombs.RemovePlayerBombs(playerId);
            _assignedSpawnPoints.Remove(playerId);
            _players.RemovePlayer(playerId);
        }
    }

    private List<SpawnPoint> GetAvailableSpawnPoints()
    {
        List<SpawnPoint> points = [];
        List<Player> players = _players.GetPlayers();
        List<IBomb> bombs = _bombs.GetBombs();
        List<Explosion> explosions = _bombs.GetExplosions();

        foreach (SpawnPoint point in _map.SpawnPoints)
        {
            if (_assignedSpawnPoints.Values.Contains(point))
                continue;

            if (!_map.IsWalkable(point.Position))
                continue;

            Position position = Map.GridToWorldPosition(point.Position, Player.PlayerSize);

            if (players.Any(p => p.State == PlayerState.Alive &&
                MathF.Abs(p.Position.X - position.X) < Player.PlayerSize &&
                MathF.Abs(p.Position.Y - position.Y) < Player.PlayerSize))
                continue;

            if (bombs.Any(b => b.Position == point.Position) ||
                explosions.Any(e => e.Contains(point.Position)))
                continue;

            points.Add(point);
        }

        return points;
    }

    public int AddBots(BotType type, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        lock (_lock)
        {
            List<SpawnPoint> points = GetAvailableSpawnPoints();

            if (points.Count < count)
                return 0;

            for (int i = 0; i < count; i++)
            {
                SpawnPoint point = points[i];
                Position position = Map.GridToWorldPosition(point.Position, Player.PlayerSize);

                Bot bot = type switch
                {
                    BotType.Hunter => BotManager.Instance.CreateHunter(position),
                    BotType.Defender => BotManager.Instance.CreateDefender(position),
                    _ => throw new ArgumentOutOfRangeException(nameof(type)),
                };

                _players.AddPlayer(bot);
                _assignedSpawnPoints.Add(bot.Id, point);
            }

            _players.ResetWinnerState();
            return count;
        }
    }

    public int RemoveBots(Guid? id = null, BotType? type = null)
    {
        lock (_lock)
        {
            List<Bot> bots = [.. _players.GetPlayers().OfType<Bot>()];
            List<Bot> selected = [.. bots.Where(b =>
                (id is null || b.Id == id.Value) &&
                (type is null || b.Name == type.Value.ToString()))];

            foreach (Bot bot in selected)
            {
                BotManager.Instance.RemoveBot(bot.Id);
                _bombs.RemovePlayerBombs(bot.Id);
                _assignedSpawnPoints.Remove(bot.Id);
                _players.RemovePlayer(bot.Id, checkForWinner: false);
            }

            if (selected.Count > 0)
                _players.ResetWinnerState();

            return selected.Count;
        }
    }

    public void SetPaused(bool paused)
    {
        lock (_lock)
            IsPaused = paused;
    }

    public void ResetMatch()
    {
        lock (_lock)
        {
            _bombs.Reset();
            _map.Reset();

            foreach (Player player in _players.GetPlayers())
            {
                SpawnPoint point = _assignedSpawnPoints[player.Id];
                Position position = Map.GridToWorldPosition(point.Position, Player.PlayerSize);
                player.Reset(position);
            }

            _players.ResetWinnerState();
            BotManager.Instance.ResetStrategies();
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
            if (IsPaused || !_players.AllPlayers.TryGetValue(playerId, out var player))
            {
                bomb = null;
                return false;
            }

            return _bombs.PlaceBomb(player, out bomb);
        }
    }

    public void DetonateBombs(Guid playerId)
    {
        lock (_lock)
        {
            if (!IsPaused)
                _bombs.DetonatePlayerBombs(playerId);
        }
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
            return !IsPaused && _bombs.RemoveBomb(bombId);
        }
    }

    public void Update(float deltaTime)
    {
        lock (_lock)
        {
            if (IsPaused)
                return;

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

    public List<IBomb> GetBombsToExplode()
    {
        return _bombs.GetBombsToExplode();
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