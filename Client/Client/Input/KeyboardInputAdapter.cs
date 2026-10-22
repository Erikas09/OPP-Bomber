namespace Bomber.Client.Input;

public class KeyboardInputAdapter : IGameInput
{
    private readonly HashSet<Keys> _pressedKeys = [];
    private bool _bombPressed;
    private bool _togglePiercing;
    private bool _toggleMegaBlast;
    private bool _toggleRemote;
    private bool _detonatePressed;

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
        else if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1)
            _togglePiercing = true;
        else if (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2)
            _toggleMegaBlast = true;
        else if (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3)
            _toggleRemote = true;
        else if (e.KeyCode == Keys.E || e.KeyCode == Keys.X)
            _detonatePressed = true;
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
        input.TogglePiercing = _togglePiercing;
        input.ToggleMegaBlast = _toggleMegaBlast;
        input.ToggleRemoteDetonation = _toggleRemote;
        input.DetonatePressed = _detonatePressed;

        _bombPressed = false;
        _togglePiercing = false;
        _toggleMegaBlast = false;
        _toggleRemote = false;
        _detonatePressed = false;

        return input;
    }

    public void Update()
    {
    }
}
