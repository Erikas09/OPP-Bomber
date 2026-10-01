namespace Bomber.Domain.Events;

public abstract record GameEvent(
    Guid Id,
    DateTimeOffset OccurredAt);