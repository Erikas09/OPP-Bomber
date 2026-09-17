namespace Bomber.Client.Models;

public sealed class PlayerStateResponse
{
    public required string Id { get; init; }
    public required float X { get; init; }
    public required float Y { get; init; }
}
