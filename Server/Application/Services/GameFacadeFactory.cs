using Bomber.Application.Publishers;
using Bomber.Domain.Entities.Bots;
using Bomber.Domain.Game;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Mediators;
using Bomber.Domain.Players;

namespace Bomber.Application.Services;

public sealed class GameFacadeFactory
{
    private readonly GameEventPublisher _eventPublisher;

    Map _map;
    PlayerSystem _players;
    BombSystem _bombs;

    public GameFacadeFactory(GameEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;

        _map = Map.CreateTestMap(Map.MapType.Classic);
        Console.WriteLine(
            $"[Flyweight] {_map.Width * _map.Height} map tiles share {MapCellFlyweightFactory.CachedCount} cell objects");
        _players = new PlayerSystem(_map);

        IBombMediator mediator = new BombMediator(_players, _map, _eventPublisher);
        _bombs = new BombSystem(_map, mediator);

        BotManager.Initialize(_map, _players.AllPlayers);
    }

    public GameFacade Create()
    {
        var game = new GameFacade(_map, _players, _bombs);

        BotManager.Instance.OnBombRequest += id => game.PlaceBomb(id, out _);
        BotManager.Instance.GetBombList = game.GetBombs;
        game.SetGame();

        return game;
    }

    public GameWorld CreateWorld()
    {
        GameWorld world = new();
        world.Add(BotManager.Instance);
        world.Add(_players);
        world.Add(_bombs);
        return world;
    }
}