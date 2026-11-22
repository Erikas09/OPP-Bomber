using Bomber.Domain.Enums;

namespace Bomber.Application.Models;

public sealed record PlayerStateResponse(
    Guid Id,
    string Name,
    float X,
    float Y,
    int Lives,
    int BombLimit,
    int ExplosionRadius,
    float Speed,
    bool IsAlive,
    bool IsMoving,
    Direction MoveDirection,
    bool IsWinner);