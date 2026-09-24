using Bomber.Domain.Common;

namespace Bomber.Domain.Maps;

public class SpawnPoint
{
    public Guid Id { get; }

    public GridPosition Position { get; }

    public int PlayerIndex { get; }

    public SpawnPoint(
        Guid id,
        GridPosition position,
        int playerIndex)
    {
        Id = id;
        Position = position;
        PlayerIndex = playerIndex;
    }
}
