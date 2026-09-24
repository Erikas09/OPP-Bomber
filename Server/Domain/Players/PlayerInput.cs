namespace Bomber.Domain.Players;

public record struct PlayerInput(
    bool Up,
    bool Down,
    bool Left,
    bool Right)
{
    public static PlayerInput None => new(false, false, false, false);
}
