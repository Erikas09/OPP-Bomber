namespace Bomber.Client.Models;

public sealed record MoveRequest(
    bool Up,
    bool Down,
    bool Left,
    bool Right);
