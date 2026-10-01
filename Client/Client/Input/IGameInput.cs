namespace Bomber.Client.Input;

public class Input
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;
    public bool bombPressed;
}

public interface IGameInput
{
    void Update();
    public Input GetInput();
}