using SDL2;

namespace Bomber.Client.Input;

public class ControllerInputAdapter : IGameInput, IDisposable
{
    const float StickDeadZone = 0.25f;

    IntPtr _controller = IntPtr.Zero;

    bool _initialized;
    bool _previousBombButtonState;

    public bool IsConnected => _controller != IntPtr.Zero;

    bool up;
    bool down;
    bool left;
    bool right;
    bool bombPressed;

    public ControllerInputAdapter()
    {
        Initialize();
    }

    void Initialize()
    {
        int result = SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER);

        if (result < 0)
        {
            throw new InvalidOperationException($"Could not initialize SDL: {SDL.SDL_GetError()}");
        }

        _initialized = true;

        FindController();
    }

    void FindController()
    {
        int joystickCount = SDL.SDL_NumJoysticks();

        if (joystickCount < 0)
        {
            return;
        }

        for (int i = 0; i < joystickCount; i++)
        {
            int isController = SDL.SDL_IsGameController(i) == 0 ? 0 : 1;

            if (isController == 0)
                continue;

            _controller = SDL.SDL_GameControllerOpen(i);

            if (_controller == IntPtr.Zero)
            {
                continue;
            }

            return;
        }
    }


    public void Update()
    {
        if (!_initialized)
            return;

        SDL.SDL_PumpEvents();

        if (_controller == IntPtr.Zero)
            return;

        UpdateMovement();

        UpdateBomb();
    }

    public Input GetInput()
    {
        return new Input()
        {
            Up = up,
            Down = down,
            Left = left,
            Right = right,
            BombPressed = bombPressed
        };
    }

    void UpdateMovement()
    {
        short x = SDL.SDL_GameControllerGetAxis(_controller, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX);

        short y = SDL.SDL_GameControllerGetAxis(_controller, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY);

        float normalizedX = NormalizeAxis(x);
        float normalizedY = NormalizeAxis(y);

        left = normalizedX < -StickDeadZone;
        right = normalizedX > StickDeadZone;
        up = normalizedY < -StickDeadZone;
        down = normalizedY > StickDeadZone;
    }

    void UpdateBomb()
    {
        bool currentButtonState = SDL.SDL_GameControllerGetButton(_controller,SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A) != 0;
        bombPressed = currentButtonState && !_previousBombButtonState;
        _previousBombButtonState = currentButtonState;
    }

    static float NormalizeAxis(short value)
    {
        if (value >= 0)
            return value / 32767f;

        return value / 32768f;
    }

    public void Dispose()
    {
        if (_controller != IntPtr.Zero)
        {
            SDL.SDL_GameControllerClose(_controller);
            _controller = IntPtr.Zero;
        }

        if (_initialized)
        {
            SDL.SDL_QuitSubSystem(SDL.SDL_INIT_GAMECONTROLLER);
            _initialized = false;
        }

        GC.SuppressFinalize(this);
    }
}
