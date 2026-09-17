namespace Bomber.Application.Models;

public sealed class GameStateResponse
{
    public required List<PlayerStateResponse> Players { get; init; }
}
