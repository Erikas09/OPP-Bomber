using Bomber.Client.Models;
using System.Diagnostics;

namespace Bomber.Client;

public sealed class ClientGameState
{
    public Guid GameId { get; private set; }

    public int MapWidth { get; private set; }

    public int MapHeight { get; private set; }

    public List<MapCellResponse> Cells { get; private set; } = [];

    public List<SpawnPointResponse> SpawnPoints { get; private set; } = [];

    public List<PlayerStateResponse> Players { get; private set; } = [];

    public List<BombResponse> Bombs { get; private set; } = [];

    public List<PowerUpResponse> PowerUps { get; private set; } = [];

    public List<ExplosionResponse> Explosions { get; private set; } = [];
    private readonly List<(ExplosionCell Cell, DateTime ExpiresAt)> _activeExplosions = [];

    public void Apply(GameStateResponse response)
    {
        MapWidth = response.MapWidth;
        MapHeight = response.MapHeight;
        Cells = response.Cells;
        SpawnPoints = response.SpawnPoints;
        Players = response.Players;
        Bombs = response.Bombs;
        PowerUps = response.PowerUps;
        Explosions = response.Explosions;
    }
    public void AddExplosion(BombExplodedResponse explosion)
    {
        DateTime expiresAt = DateTime.UtcNow.AddMilliseconds(400);

        foreach (ExplosionCell cell in explosion.Cells)
            _activeExplosions.Add((cell, expiresAt));
    }

    public List<ExplosionCell> GetActiveExplosionCells()
    {
        DateTime now = DateTime.UtcNow;

        _activeExplosions.RemoveAll(e => e.ExpiresAt <= now);

        return [.. _activeExplosions.Select(e => e.Cell)];
    }
}
