namespace Bomber.Application.Models;

public sealed record BombResponse(
    Guid Id,
    Guid OwnerId,
    int X,
    int Y,
    int Radius);