namespace Bomber.Application.Models;

public sealed record MapCellResponse(
    Guid Id,
    int X,
    int Y,
    MapCellType Type);
