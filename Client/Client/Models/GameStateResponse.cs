namespace Bomber.Client.Models;

public sealed record GameStateResponse(
    Guid GameId,
    int MapWidth,
    int MapHeight,
    List<MapCellResponse> Cells,
    List<SpawnPointResponse> SpawnPoints,
    List<PlayerStateResponse> Players,
    List<BombResponse> Bombs,
    List<PowerUpResponse> PowerUps,
    List<ExplosionResponse> Explosions);