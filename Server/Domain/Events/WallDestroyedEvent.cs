using Bomber.Domain.Common;

namespace Bomber.Domain.Events;

public sealed record WallDestroyedEvent(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid WallId,
    Guid BombId,
    Guid CausedByPlayerId,
    GridPosition Position)
    : GameEvent(Id, OccurredAt);