using Bomber.Domain.Enums;

namespace Bomber.Application.Models;

public sealed record PowerUpResponse(
    Guid Id,
    int X,
    int Y,
    PowerUpType Type);
