namespace Bomber.Application.Models;

public sealed record GameStateResponse(
    int MapWidth,
    int MapHeight,
    List<MapCellResponse> Cells,
    List<SpawnPointResponse> SpawnPoints,
    List<PlayerStateResponse> Players,
    List<BombResponse> Bombs,
    List<PowerUpResponse> PowerUps);