using Bomber.Domain.Common.Interfaces;
using Bomber.Domain.Players;

namespace Bomber.Application.Iterators;

public sealed class PlayerIterator : IIterator<Player>
{
    private readonly Dictionary<Guid, Player> _players;
    private Dictionary<Guid, Player>.ValueCollection.Enumerator _enumerator;

    private Player? _current;
    private bool _hasNext;

    public PlayerIterator(Dictionary<Guid, Player> players)
    {
        _players = players;
        _enumerator = _players.Values.GetEnumerator();

        Advance();
    }

    public bool HasNext()
    {
        return _hasNext;
    }

    public Player Next()
    {
        if (!_hasNext || _current is null)
            throw new InvalidOperationException("No more players.");

        Player player = _current;

        Advance();

        return player;
    }

    public void Reset()
    {
        _enumerator = _players.Values.GetEnumerator();

        Advance();
    }

    private void Advance()
    {
        _hasNext = _enumerator.MoveNext();

        if (_hasNext)
            _current = _enumerator.Current;
        else
            _current = null;
    }
}
