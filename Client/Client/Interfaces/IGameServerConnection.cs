using Bomber.Client.Models;

namespace Bomber.Client.Interfaces;

public interface IGameServerConnection
{
    event Action<Guid>? PlayerAssigned;

    event Action<GameStateResponse>? GameStateUpdated;

    event Action<BombExplodedResponse>? BombExploded;

    Task ConnectAsync();

    Task DisconnectAsync();

    Task SendMovementAsync(MoveRequest request);

    Task SendPlaceBombAsync();

    Task SendUndoAsync();

    Task<bool> TogglePiercingAsync();

    Task<bool> ToggleMegaBlastAsync();

    Task<bool> ToggleRemoteDetonationAsync();

    Task SendDetonateBombsAsync();
}
