using Bomber.Application.Publishers;
using Bomber.Domain.Entities.Bots;
using Bomber.Domain.Maps;
using Bomber.Domain.Players;

namespace Bomber.Application.Services;

public sealed class GameFacadeFactory
{
    private readonly GameEventPublisher _eventPublisher;

    public GameFacadeFactory(GameEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public GameFacade Create()
    {
        var map = Map.CreateTestMap(Map.MapType.Classic);
        var players = new PlayerSystem(map);
        var bombs = new BombSystem(map, players, _eventPublisher);
        BotManager.Initialize(map, players.AllPlayers);

        var game = new GameFacade(map, players, bombs);

        BotManager.Instance.OnBombRequest += id => game.PlaceBomb(id, out _);
        BotManager.Instance.GetBombList = game.GetBombs;
        game.SetGame();

        return game;
    }
}