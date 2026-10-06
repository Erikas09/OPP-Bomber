namespace Bomber.Client.Models;

public sealed record ExplosionResponse(
    Guid Id,
    Guid BombId,
    Guid OwnerId,
    List<GridPosition> Cells);