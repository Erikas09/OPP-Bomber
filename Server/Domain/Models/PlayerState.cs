namespace Bomber.Domain.Models;

public sealed class PlayerState
{
    public required string Id { get; init; }

    public float X { get; set; }

    public float Y { get; set; }
}
