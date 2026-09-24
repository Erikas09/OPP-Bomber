using Bomber.Client.Models;
using Bomber.Client.Network;
using Bomber.Client.Rendering;

namespace Bomber.Client;

public partial class Game : Form
{
    private readonly GameServerConnection _serverConnection;
    private readonly ClientGameState _gameState;
    private readonly GameRenderer _renderer;

    private readonly System.Windows.Forms.Timer _inputTimer;

    private Guid? _localPlayerId;

    private bool _up;
    private bool _down;
    private bool _left;
    private bool _right;

    private bool _sendingInput;

    public Game()
    {
        InitializeComponent();

        KeyPreview = true;

        _gameState = new ClientGameState();
        _renderer = new GameRenderer();

        _serverConnection = new GameServerConnection("https://localhost:7064");

        _serverConnection.PlayerAssigned += OnPlayerAssigned;

        _serverConnection.GameStateUpdated += OnGameStateUpdated;

        KeyDown += Game_KeyDown;
        KeyUp += Game_KeyUp;

        gamePanel.Paint += GamePanel_Paint;

        Shown += Game_Shown;
        FormClosed += Game_FormClosed;

        _inputTimer = new System.Windows.Forms.Timer
        {
            Interval = 50
        };

        _inputTimer.Tick += InputTimer_Tick;
    }

    private async void Game_Shown(
        object? sender,
        EventArgs e)
    {
        await _serverConnection.ConnectAsync();

        _inputTimer.Start();
    }

    private void Game_KeyDown(
        object? sender,
        KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.W:
                _up = true;
                break;

            case Keys.S:
                _down = true;
                break;

            case Keys.A:
                _left = true;
                break;

            case Keys.D:
                _right = true;
                break;
        }
    }

    private void Game_KeyUp(
        object? sender,
        KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.W:
                _up = false;
                break;

            case Keys.S:
                _down = false;
                break;

            case Keys.A:
                _left = false;
                break;

            case Keys.D:
                _right = false;
                break;
        }
    }

    private async void InputTimer_Tick(
        object? sender,
        EventArgs e)
    {
        if (_sendingInput)
            return;

        try
        {
            _sendingInput = true;

            MoveRequest request =
                new(
                    _up,
                    _down,
                    _left,
                    _right);

            await _serverConnection.SendMovementAsync(request);
        }
        finally
        {
            _sendingInput = false;
        }
    }

    private void OnPlayerAssigned(
        Guid playerId)
    {
        _localPlayerId = playerId;
    }

    private void OnGameStateUpdated(
        GameStateResponse response)
    {
        if (IsDisposed)
            return;

        if (InvokeRequired)
        {
            BeginInvoke(() => OnGameStateUpdated(response));

            return;
        }

        _gameState.Apply(response);

        gamePanel.Invalidate();
    }

    private void GamePanel_Paint(
        object? sender,
        PaintEventArgs e)
    {
        _renderer.Draw(
            e.Graphics,
            _gameState,
            _localPlayerId);
    }

    private async void Game_FormClosed(
        object? sender,
        FormClosedEventArgs e)
    {
        _inputTimer.Stop();

        await _serverConnection.DisconnectAsync();
    }
}