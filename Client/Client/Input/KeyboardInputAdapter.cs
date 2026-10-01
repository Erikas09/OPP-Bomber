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

    private void OnKeyDown(
        object? sender,
        KeyEventArgs e)
    {
        _pressedKeys.Add(e.KeyCode);
    }

    private void OnKeyUp(
        object? sender,
        KeyEventArgs e)
    {
        _pressedKeys.Remove(e.KeyCode);

        if (e.KeyCode == Keys.Space)
            _bombPressed = true;
    }

    public Input GetInput()
    {
        Input input = new();

        input.up = _pressedKeys.Contains(Keys.W);
        input.down = _pressedKeys.Contains(Keys.S);
        input.left = _pressedKeys.Contains(Keys.A);
        input.right = _pressedKeys.Contains(Keys.D);
        input.bombPressed = _bombPressed;
        _bombPressed = false;

        return input;
    }

    public void Update()
    {
    }
}
