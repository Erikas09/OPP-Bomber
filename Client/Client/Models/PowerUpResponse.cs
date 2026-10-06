namespace Bomber.Client.Models;

public sealed record PowerUpResponse(
    Guid Id,
    int X,
    int Y,
    PowerUpType Type);