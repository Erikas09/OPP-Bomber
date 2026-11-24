using Bomber.Application.Publishers;
using Bomber.Application.Services;
using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Events;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;

namespace Bomber.Domain.Mediators
{
    public class BombMediator : IBombMediator
    {
        private readonly PlayerSystem _players;
        private readonly Map _map;
        private readonly GameEventPublisher _eventPublisher;

        public BombMediator(PlayerSystem players, Map map, GameEventPublisher eventPublisher)
        {
            _players = players;
            _map = map;
            _eventPublisher = eventPublisher;
        }

        public void HandleExplosion(IBomb bomb, Explosion explosion, List<GridPosition> affectedCells)
        {
            List<GameEvent> events = [];

            DateTimeOffset occurredAt = DateTimeOffset.UtcNow;

            // notify that bomb exploded
            events.Add(
                new BombExplodedEvent(
                    Guid.NewGuid(),
                    occurredAt,
                    bomb.Id,
                    bomb.OwnerId,
                    bomb.Position,
                    [.. affectedCells]));

            // handle wall destruction
            foreach (GridPosition cell in affectedCells)
            {
                MapCell mapCell = _map.GetCell(cell);

                if (mapCell is not DestructibleWall)
                    continue;

                Guid wallId = _map.GetTile(cell).Id;

                _map.DestroyWall(cell);

                events.Add(
                    new WallDestroyedEvent(
                        Guid.NewGuid(),
                        occurredAt,
                        wallId,
                        bomb.Id,
                        bomb.OwnerId,
                        cell));
            }

            // handle player damage
            DamagePlayers(bomb, explosion, events);

            // publish all events
            foreach (GameEvent gameEvent in events)
                _eventPublisher.Notify(gameEvent);

        }
        void DamagePlayers(IBomb bomb, Explosion explosion, List<GameEvent> events)
        {
            foreach (Player player in _players.AllPlayers.Values)
            {
                if (player.State == PlayerState.Dead)
                    continue;

                GridPosition playerCell = Map.WorldToGridPosition(player.Position, Player.PlayerSize);

                if (!explosion.Contains(playerCell))
                    continue;

                int livesBefore = player.Lives;
                PlayerState stateBefore = player.State;

                player.TakeDamage();

                if (player.Lives < livesBefore)
                    events.Add(
                        new PlayerDamagedEvent(
                            Guid.NewGuid(),
                            explosion.CreatedAt,
                            player.Id,
                            bomb.Id,
                            bomb.OwnerId,
                            player.Lives));

                if (stateBefore != PlayerState.Dead && player.State == PlayerState.Dead)
                    events.Add(
                        new PlayerDiedEvent(
                            Guid.NewGuid(),
                            explosion.CreatedAt,
                            player.Id,
                            bomb.Id,
                            bomb.OwnerId));
            }
        }
    }
}
