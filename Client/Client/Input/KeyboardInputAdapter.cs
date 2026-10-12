namespace Bomber.Client.Input;

public class KeyboardInputAdapter : IGameInput
{
    private readonly HashSet<Keys> _pressedKeys = [];
    private bool _bombPressed;

    public KeyboardInputAdapter(Form form)
    {
        form.KeyDown += OnKeyDown;
        form.KeyUp += OnKeyUp;
    }

    void OnKeyDown(object? sender, KeyEventArgs e)
    {
        _pressedKeys.Add(e.KeyCode);
    }

    void OnKeyUp(object? sender, KeyEventArgs e)
    {
        _pressedKeys.Remove(e.KeyCode);

        if (e.KeyCode == Keys.Space)
            _bombPressed = true;
    }

    public Input GetInput()
    {
        Input input = new();

        input.Up = _pressedKeys.Contains(Keys.W) || _pressedKeys.Contains(Keys.Up);
        input.Down = _pressedKeys.Contains(Keys.S) || _pressedKeys.Contains(Keys.Down);
        input.Left = _pressedKeys.Contains(Keys.A) || _pressedKeys.Contains(Keys.Left);
        input.Right = _pressedKeys.Contains(Keys.D) || _pressedKeys.Contains(Keys.Right);
        input.BombPressed = _bombPressed;
        input.Undo = _pressedKeys.Contains(Keys.Z);
        _bombPressed = false;

        return input;
    }

    public void Update()
    {
    }
}
