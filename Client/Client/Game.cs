using Bomber.Client.Contexts;
using Bomber.Client.Input;
using Bomber.Client.Interfaces;
using Bomber.Client.Models;
using Bomber.Client.Network;
using Bomber.Client.Rendering;
using System.Diagnostics;

namespace Bomber.Client;

public partial class Game : Form
{
    private IGameServerConnection _serverConnection;
    private readonly GameServerConnection _realServerConnection;
    private readonly GameServerConnectionProxy _proxyConnection;
    private bool _proxyEnabled;

    private long _directRequests;
    private long _proxyRequests;
    private long _directElapsedTicks;
    private long _proxyElapsedTicks;
    private long? _directMemoryBytes;
    private long? _proxyMemoryBytes;
    private readonly ClientGameState _gameState;
    private readonly GameRenderer _renderer;

    private readonly System.Windows.Forms.Timer _renderTimer = new() { Interval = 50 };
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

        _realServerConnection = new GameServerConnection("https://localhost:7064");
        _proxyConnection = new GameServerConnectionProxy(_realServerConnection);
        _serverConnection = _realServerConnection;

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
        KeyUp += Game_KeyUp;

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

        UpdateTitle();
        _inputTimer.Start();
    }

    private bool _piercingActive;
    private bool _megaBlastActive;
    private bool _remoteActive;

    private void UpdateTitle()
    {
        Text = $"Bomber | [1] Piercing: {(_piercingActive ? "ON" : "OFF")} | [2] MegaBlast: {(_megaBlastActive ? "ON" : "OFF")} | [3] Remote: {(_remoteActive ? "ON" : "OFF")} | [E] Detonate | [F9] Proxy: {(_proxyEnabled ? "ON" : "OFF")} | [F10] Stats";
    }

    async void UpdateInput(object? sender, EventArgs e)
    {
        if (_sendingInput)
            return;

        _input.Update();

        Input.Input input = _input.GetInput();

        if (input.TogglePiercing)
        {
            _piercingActive = await _serverConnection.TogglePiercingAsync();
            UpdateTitle();
        }

        if (input.ToggleMegaBlast)
        {
            _megaBlastActive = await _serverConnection.ToggleMegaBlastAsync();
            UpdateTitle();
        }

        if (input.ToggleRemoteDetonation)
        {
            _remoteActive = await _serverConnection.ToggleRemoteDetonationAsync();
            UpdateTitle();
        }

        if (input.DetonatePressed)
        {
            _ = _serverConnection.SendDetonateBombsAsync();
        }

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

            IGameServerConnection connection = _serverConnection;
            bool proxyEnabled = _proxyEnabled;
            long startedAt = Stopwatch.GetTimestamp();

            try
            {
                await connection.SendMovementAsync(request);
            }
            finally
            {
                long elapsedTicks = Stopwatch.GetTimestamp() - startedAt;

                if (proxyEnabled)
                {
                    _proxyRequests++;
                    _proxyElapsedTicks += elapsedTicks;
                }
                else
                {
                    _directRequests++;
                    _directElapsedTicks += elapsedTicks;
                }
            }
        }
        finally
        {
            _sendingInput = false;
        }
    }

    private void Game_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F9)
        {
            _proxyEnabled = !_proxyEnabled;

            if (_proxyEnabled)
            {
                _proxyConnection.ResetMovementCache();
                _serverConnection = _proxyConnection;
            }
            else
                _serverConnection = _realServerConnection;

            UpdateTitle();
        }
        else if (e.KeyCode == Keys.F10)
            ShowConnectionStatistics();
    }

    private void ShowConnectionStatistics()
    {
        long memoryBytes = GC.GetTotalMemory(false);

        if (_proxyEnabled)
            _proxyMemoryBytes = memoryBytes;
        else
            _directMemoryBytes = memoryBytes;

        double directMilliseconds = _directElapsedTicks * 1000.0 / Stopwatch.Frequency;
        double proxyMilliseconds = _proxyElapsedTicks * 1000.0 / Stopwatch.Frequency;

        double directAverage = _directRequests > 0
            ? directMilliseconds / _directRequests
            : 0;

        double proxyAverage = _proxyRequests > 0
            ? proxyMilliseconds / _proxyRequests
            : 0;

        string directMemory = _directMemoryBytes.HasValue
            ? $"{_directMemoryBytes.Value / 1024.0 / 1024.0:F2} MB"
            : "not measured";

        string proxyMemory = _proxyMemoryBytes.HasValue
            ? $"{_proxyMemoryBytes.Value / 1024.0 / 1024.0:F2} MB"
            : "not measured";

        string statistics =
            $"Direct connection:\n" +
            $"Movement calls: {_directRequests}\n" +
            $"Average call time: {directAverage:F3} ms\n" +
            $"Managed memory: {directMemory}\n\n" +
            $"Proxy connection:\n" +
            $"Movement calls: {_proxyRequests}\n" +
            $"Forwarded: {_proxyConnection.ForwardedRequests}\n" +
            $"Skipped: {_proxyConnection.SkippedRequests}\n" +
            $"Average call time: {proxyAverage:F3} ms\n" +
            $"Managed memory: {proxyMemory}\n\n" +
            $"Current mode: {(_proxyEnabled ? "Proxy" : "Direct")}";

        MessageBox.Show(
            this,
            statistics,
            "Proxy measurements",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnPlayerAssigned(
        Guid playerId)
    {
        _localPlayerId = playerId;
    }

    private void OnGameStateUpdated(GameStateResponse response)
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
