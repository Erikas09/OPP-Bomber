using Bomber.Domain.Common;

namespace Bomber.Application.Models;

public sealed record ExplosionResponse(
    Guid Id,
    Guid BombId,
    Guid OwnerId,
    List<GridPosition> Cells);