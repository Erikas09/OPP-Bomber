using Bomber.Domain.Common;

namespace Bomber.Domain.Bombs;

public sealed class Explosion
{
    private readonly List<GridPosition> _cells;

    public Guid Id { get; }

    public Guid BombId { get; }

    public Guid OwnerId { get; }

    public GridPosition Origin { get; }

    public List<GridPosition> Cells => _cells;

    public DateTimeOffset CreatedAt { get; }

    public float DurationSeconds { get; }

    public Explosion(
        Guid id,
        Guid bombId,
        Guid ownerId,
        GridPosition origin,
        List<GridPosition> cells,
        DateTimeOffset createdAt,
        float durationSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(durationSeconds, 0f);

        Id = id;
        BombId = bombId;
        OwnerId = ownerId;
        Origin = origin;
        _cells = [.. cells];
        CreatedAt = createdAt;
        DurationSeconds = durationSeconds;
    }

    public bool Contains(GridPosition position)
    {
        return _cells.Contains(position);
    }

    public bool IsExpired(DateTimeOffset now)
    {
        return now >= CreatedAt.AddSeconds(DurationSeconds);
    }
}
