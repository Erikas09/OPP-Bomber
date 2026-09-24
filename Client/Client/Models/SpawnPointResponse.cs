namespace Bomber.Client.Models;

public sealed record SpawnPointResponse(
    Guid Id,
    int X,
    int Y,
    int PlayerIndex);
