using Bomber.Domain.Enums;
using Bomber.Domain.Maps;
using Bomber.Domain.Players;

namespace Bomber.Domain.Games;

public class Game
{
    private readonly List<Player> _players = [];

    public Guid Id { get; }

    public Map Map { get; }

    public IReadOnlyCollection<Player> Players => _players;

    public GameState State { get; private set; }

    public Game(
        Guid id,
        Map map)
    {
        Id = id;
        Map = map;
        State = GameState.NotStarted;
    }

    public void AddPlayer(Player player)
    {
        if (State != GameState.NotStarted)
            return;

        if (_players.Count >= 4)
            return;

        _players.Add(player);
    }

    public void RemovePlayer(Guid playerId)
    {
        Player? player = _players.FirstOrDefault(
            player => player.Id == playerId);

        if (player is not null)
            _players.Remove(player);
    }

    public Player? GetPlayer(Guid playerId)
    {
        return _players.FirstOrDefault(
            player => player.Id == playerId);
    }

    public void Start()
    {
        if (_players.Count < 2)
            return;

        State = GameState.InProgress;
    }
}
