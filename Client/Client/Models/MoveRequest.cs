namespace Bomber.Client.Models;

public sealed class MoveRequest
{
    public required bool Up {  get; set; }
    public required bool Down { get; set; }
    public required bool Left { get; set; }
    public required bool Right { get; set; }
}
