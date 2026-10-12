using Bomber.Client.Contexts;
using Bomber.Client.Input;
using Bomber.Client.Models;
using Bomber.Client.Network;
using Bomber.Client.Rendering;

namespace Bomber.Client;

public partial class Game : Form
{
    private readonly GameServerConnection _serverConnection;
    private readonly ClientGameState _gameState;
    private readonly GameRenderer _renderer;

    private readonly System.Windows.Forms.Timer _renderTimer =
        new() { Interval = 50 };
    private readonly System.Windows.Forms.Timer _inputTimer;
    private readonly IGameInput _input;
    private Guid? _localPlayerId;
    private bool _sendingInput;

    public Game()
    {
        InitializeComponent();

        KeyPreview = true;

        _gameState = new ClientGameState();

        _renderer = new GameRenderer(new SystemDrawingContext());

        _serverConnection = new GameServerConnection("https://localhost:7064");

        var controller = new ControllerInputAdapter();

        if (controller.IsConnected)
        {
            _input = controller;
            Text = "Controller";
        }
        else
        {
            controller.Dispose();
            _input = new KeyboardInputAdapter(this);
            Text = "Keyboard";
        }

        _serverConnection.PlayerAssigned += OnPlayerAssigned;

        _serverConnection.GameStateUpdated += OnGameStateUpdated;

        _serverConnection.BombExploded += OnBombExploded;

        gamePanel.Paint += GamePanel_Paint;

        Shown += Game_Shown;
        FormClosed += Game_FormClosed;

        _inputTimer = new System.Windows.Forms.Timer
        {
            Interval = 50
        };

        _inputTimer.Tick += UpdateInput;

        _renderTimer.Tick += (_, _) =>
            gamePanel.Invalidate();

        _renderTimer.Start();
    }

    private async void Game_Shown(
        object? sender,
        EventArgs e)
    {
        await _serverConnection.ConnectAsync();

        _inputTimer.Start();
    }

    async void UpdateInput(object? sender, EventArgs e)
    {
        if (_sendingInput)
            return;

        _input.Update();

        Input.Input input = _input.GetInput();

        if (input.BombPressed)
        {
            _ = PlaceBombAsync();
        }

        if (input.Undo)
        {
            _ = UndoAsync();
        }

        try
        {
            _sendingInput = true;

            MoveRequest request =
                new(
                    input.Up,
                    input.Down,
                    input.Left,
                    input.Right);

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
            BeginInvoke(() =>
                OnGameStateUpdated(response));

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

    private async Task PlaceBombAsync()
    {
        try
        {
            await _serverConnection.SendPlaceBombAsync();
        }
        catch
        {
            // Ignoruojam. Vistiek kitas tick atnaujins būseną
        }
    }

    private async Task UndoAsync()
    {
        try
        {
            await _serverConnection.SendUndoAsync();
        }
        catch
        {

        }
    }

    private void OnBombExploded(
        BombExplodedResponse explosion)
    {
        if (IsDisposed)
            return;

        if (InvokeRequired)
        {
            BeginInvoke(() =>
                OnBombExploded(explosion));

            return;
        }

        _gameState.AddExplosion(explosion);

        gamePanel.Invalidate();
    }
}
