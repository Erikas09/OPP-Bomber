using Bomber.Domain.Common;

namespace Bomber.Domain.Events;

public sealed record BombExplodedEvent(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid BombId,
    Guid OwnerId,
    GridPosition Origin,
    List<GridPosition> AffectedCells)
    : GameEvent(Id, OccurredAt);