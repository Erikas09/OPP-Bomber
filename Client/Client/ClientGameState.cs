using Bomber.Client.Models;

namespace Bomber.Client;

public sealed class ClientGameState
{
    public Guid GameId { get; private set; }

    public int MapWidth { get; private set; }

    public int MapHeight { get; private set; }

    public List<MapCellResponse> Cells { get; private set; } = [];

    public List<SpawnPointResponse> SpawnPoints { get; private set; } = [];

    public List<PlayerStateResponse> Players { get; private set; } = [];

    public void Apply(GameStateResponse response)
    {
        GameId = response.GameId;
        MapWidth = response.MapWidth;
        MapHeight = response.MapHeight;

        Cells = response.Cells;
        SpawnPoints = response.SpawnPoints;
        Players = response.Players;
    }
}
