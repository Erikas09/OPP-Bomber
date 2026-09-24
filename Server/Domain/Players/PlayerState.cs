namespace Bomber.Domain.Players;

public sealed class PlayerState
{
    public required string Id { get; init; }

    public float X { get; set; }

    public float Y { get; set; }
}
