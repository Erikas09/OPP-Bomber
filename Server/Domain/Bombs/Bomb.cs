using Bomber.Domain.Common;

namespace Bomber.Domain.Bombs;

/// <summary>
/// Concrete component — the basic bomb with default behavior.
/// This is the primary object that decorators will wrap.
/// </summary>
public class Bomb : IBomb
{
    public Guid Id { get; }

    public Guid OwnerId { get; }

    public GridPosition Position { get; }

    public int Radius { get; }

    public DateTimeOffset PlacedAt { get; }

    public int FuseSeconds { get; }

    public virtual bool IsPiercing => false;

    public virtual bool IsRemoteDetonation => false;

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

    public virtual bool ShouldExplode(DateTimeOffset now)
    {
        return now >= PlacedAt.AddSeconds(FuseSeconds);
    }

    public virtual string GetDescription()
    {
        return "Basic Bomb";
    }

    public virtual bool TryDetonate()
    {
        return false;
    }
}