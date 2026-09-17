using Bomber.Client.Models;
using Bomber.Client.Network;

namespace Bomber.Client
{
    public partial class Game : Form
    {
        private const int PlayerSize = 30;

        private readonly GameServerConnection _serverConnection;
        private readonly System.Windows.Forms.Timer _movementTimer;

        private bool _up;
        private bool _down;
        private bool _left;
        private bool _right;

        private bool _sendingMovement;

        private string? _localPlayerId;
        private GameStateResponse? _gameState;

        public Game()
        {
            InitializeComponent();

            KeyPreview = true;

            _serverConnection =
                new GameServerConnection("https://localhost:7064");

            _serverConnection.PlayerAssigned += OnPlayerAssigned;
            _serverConnection.GameStateUpdated += OnGameStateUpdated;

            KeyDown += Game_KeyDown;
            KeyUp += Game_KeyUp;

            gamePanel.Paint += GamePanel_Paint;

            Shown += Game_Shown;
            FormClosed += Game_FormClosed;

            _movementTimer = new System.Windows.Forms.Timer
            {
                Interval = 50
            };

            _movementTimer.Tick += MovementTimer_Tick;
        }

        private async void Game_Shown(
            object? sender,
            EventArgs e)
        {
            try
            {
                await _serverConnection.ConnectAsync();

                _movementTimer.Start();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.ToString(),
                    "Could not connect to server",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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

        private async void MovementTimer_Tick(
            object? sender,
            EventArgs e)
        {
            if (_sendingMovement)
                return;

            if (_localPlayerId is null)
                return;

            if (!_up &&
                !_down &&
                !_left &&
                !_right)
            {
                return;
            }

            try
            {
                _sendingMovement = true;

                MoveRequest request = new()
                {
                    Up = _up,
                    Down = _down,
                    Left = _left,
                    Right = _right
                };

                await _serverConnection.SendMovementAsync(request);
            }
            catch (Exception exception)
            {
                Console.WriteLine(
                    $"Failed to send movement: {exception.Message}");
            }
            finally
            {
                _sendingMovement = false;
            }
        }

        private void OnPlayerAssigned(string playerId)
        {
            _localPlayerId = playerId;
        }

        private void OnGameStateUpdated(
            GameStateResponse gameState)
        {
            if (IsDisposed)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(() =>
                    OnGameStateUpdated(gameState));

                return;
            }

            _gameState = gameState;

            gamePanel.Invalidate();
        }

        private void GamePanel_Paint(
            object? sender,
            PaintEventArgs e)
        {
            if (_gameState is null)
                return;

            foreach (var player in _gameState.Players)
            {
                Brush brush =
                    player.Id == _localPlayerId
                        ? Brushes.Blue
                        : Brushes.Red;

                e.Graphics.FillRectangle(
                    brush,
                    player.X,
                    player.Y,
                    PlayerSize,
                    PlayerSize);
            }
        }

        private async void Game_FormClosed(
            object? sender,
            FormClosedEventArgs e)
        {
            _movementTimer.Stop();

            try
            {
                await _serverConnection.DisconnectAsync();
            }
            catch
            {
            }
        }
    }
}