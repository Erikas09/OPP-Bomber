 using Bomber.Client.Input;

public sealed class KeyboardInputAdapter : IGameInput
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

        input.up = _pressedKeys.Contains(Keys.W) || _pressedKeys.Contains(Keys.Up);
        input.down = _pressedKeys.Contains(Keys.S) || _pressedKeys.Contains(Keys.Down);
        input.left = _pressedKeys.Contains(Keys.A) || _pressedKeys.Contains(Keys.Left);
        input.right = _pressedKeys.Contains(Keys.D) || _pressedKeys.Contains(Keys.Right);
        input.bombPressed = _bombPressed;
        _bombPressed = false;

        return input;
    }

    public void Update()
    {
    }
}
