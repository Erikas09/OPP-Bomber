using Bomber.Domain.Entities.Bots;
using Bomber.Domain.Enums;
using Bomber.Domain.Players;

namespace Bomber.Application.Services;

public sealed class GameAdministrationService
{
    private readonly GameFacade _game;

    public GameAdministrationService(GameFacade game)
    {
        _game = game;
    }

    public string Pause()
    {
        lock (_game.SyncRoot)
        {
            if (_game.IsPaused)
                return "Game is already paused.";

            _game.SetPaused(true);
            return "Game paused.";
        }
    }

    public string Resume()
    {
        lock (_game.SyncRoot)
        {
            if (!_game.IsPaused)
                return "Game is already running.";

            _game.SetPaused(false);
            return "Game resumed.";
        }
    }

    public string Reset()
    {
        _game.ResetMatch();
        return "Game reset. Players and bots retained. Pause state unchanged.";
    }

    public string Status()
    {
        lock (_game.SyncRoot)
        {
            List<Player> players = _game.GetPlayers();
            int bots = players.Count(p => p is Bot);
            int alive = players.Count(p => p.State == PlayerState.Alive);
            string state = _game.IsPaused ? "Paused" : "Running";

            return $"{state} | Humans: {players.Count - bots} | Bots: {bots} | " +
                $"Alive: {alive} | Capacity: {_game.GetMap().SpawnPoints.Count} | " +
                $"Bombs: {_game.GetBombs().Count} | Explosions: {_game.GetExplosions().Count}";
        }
    }

    public string AddBots(BotType type, int count)
    {
        int added = _game.AddBots(type, count);

        if (added == 0)
            return "Not enough available spawn points. No bots added.";

        return $"Added {added} {type} bot(s).";
    }

    public string RemoveBots(Guid? id = null, BotType? type = null)
    {
        int removed = _game.RemoveBots(id, type);
        return $"Removed {removed} bot(s).";
    }

    public string ListBots()
    {
        lock (_game.SyncRoot)
        {
            List<Bot> bots = [.. _game.GetPlayers().OfType<Bot>()];

            if (bots.Count == 0)
                return "No bots in the game.";

            return string.Join(Environment.NewLine, bots.Select(b =>
                $"{b.Id} | {b.Name} | {b.State} | Lives: {b.Lives}"));
        }
    }
}
