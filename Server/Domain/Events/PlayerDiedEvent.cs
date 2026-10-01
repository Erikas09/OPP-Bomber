namespace Bomber.Domain.Events;

public sealed record PlayerDiedEvent(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid PlayerId,
    Guid BombId,
    Guid CausedByPlayerId)
    : GameEvent(Id, OccurredAt);
