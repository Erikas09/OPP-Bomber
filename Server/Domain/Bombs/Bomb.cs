using Bomber.Domain.Common;

namespace Bomber.Domain.Bombs;

public sealed class Bomb
{
    public Guid Id { get; }

    public Guid OwnerId { get; }

    public GridPosition Position { get; }

    public int Radius { get; }

    public DateTimeOffset PlacedAt { get; }

    public int FuseSeconds { get; }

    public Bomb(
        Guid id,
        Guid ownerId,
        GridPosition position,
        int radius,
        int fuseSeconds)
    {
        Id = id;
        OwnerId = ownerId;
        Position = position;
        Radius = radius;
        FuseSeconds = fuseSeconds;
        PlacedAt = DateTimeOffset.UtcNow;
    }

    public bool ShouldExplode(DateTimeOffset now)
    {
        return now >= PlacedAt.AddSeconds(FuseSeconds);
    }
}