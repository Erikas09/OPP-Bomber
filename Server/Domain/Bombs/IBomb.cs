using Bomber.Domain.Common;

namespace Bomber.Domain.Bombs;

/// <summary>
/// Component interface for the Decorator pattern.
/// Declares the common contract for both the primary bomb component
/// and all optional decorator layers.
/// </summary>
public interface IBomb
{
    Guid Id { get; }

    Guid OwnerId { get; }

    GridPosition Position { get; }

    int Radius { get; }

    DateTimeOffset PlacedAt { get; }

    int FuseSeconds { get; }

    /// <summary>
    /// Determines whether the bomb should explode at the given time.
    /// Decorators can override this to change detonation behavior.
    /// </summary>
    bool ShouldExplode(DateTimeOffset now);

    /// <summary>
    /// Whether the explosion pierces through destructible walls
    /// instead of stopping at them.
    /// </summary>
    bool IsPiercing { get; }

    /// <summary>
    /// Whether this bomb supports manual (remote) detonation.
    /// </summary>
    bool IsRemoteDetonation { get; }

    /// <summary>
    /// Returns a description of all active modifications on this bomb.
    /// Useful for logging and debugging the decorator chain.
    /// </summary>
    string GetDescription();

    /// <summary>
    /// Attempts to trigger a manual detonation if supported by the bomb or its decorators.
    /// Returns true if detonation was handled.
    /// </summary>
    bool TryDetonate();
}
