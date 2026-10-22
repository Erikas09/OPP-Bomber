namespace Bomber.Client.Input;

public class Input
{
    public bool Up { set; get; }
    public bool Down { set; get; }
    public bool Left { set; get; }
    public bool Right { set; get; }
    public bool BombPressed { set; get; }
    public bool Undo { set; get; }
    public bool TogglePiercing { set; get; }
    public bool ToggleMegaBlast { set; get; }
    public bool ToggleRemoteDetonation { set; get; }
    public bool DetonatePressed { set; get; }
}

public interface IGameInput
{
    void Update();
    public Input GetInput();
}