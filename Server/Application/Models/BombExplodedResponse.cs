namespace Bomber.Application.Models;

public sealed record ExplosionCell(
    int X,
    int Y);

public sealed record BombExplodedResponse(
    Guid BombId,
    List<ExplosionCell> Cells);