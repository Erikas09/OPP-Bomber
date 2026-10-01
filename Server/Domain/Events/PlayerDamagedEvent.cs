namespace Bomber.Domain.Events;

public sealed record PlayerDamagedEvent(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid PlayerId,
    Guid BombId,
    Guid CausedByPlayerId,
    int LivesRemaining)
    : GameEvent(Id, OccurredAt);