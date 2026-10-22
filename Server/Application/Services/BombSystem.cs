using Bomber.Application.Commands;
using Bomber.Application.Publishers;
using Bomber.Domain.Bombs;
using Bomber.Domain.Common;
using Bomber.Domain.Enums;
using Bomber.Domain.Events;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Application.Services
{
    public sealed class BombSystem
    {
        const float ExplosionDurationSeconds = 0.5f;

        // ── Uses IBomb interface to support both plain bombs and decorated bombs ──
        private readonly Dictionary<Guid, IBomb> _bombs = [];
        private readonly Dictionary<Guid, Explosion> _explosions = [];
        private readonly List<PlaceBombCommand> _commands = [];

        readonly PlayerSystem _players;
        readonly Map _map;
        readonly GameEventPublisher _eventPublisher;

        public BombSystem(Map map, PlayerSystem players, GameEventPublisher eventPublisher)
        {
            _map = map;
            _players = players;
            _eventPublisher = eventPublisher;
        }

        public void Update(float deltaTime)
        {
            List<IBomb> bombs = new(_bombs.Values);

            foreach (var bomb in bombs)
                UpdateBomb(bomb);

            List<Explosion> explosions = new(_explosions.Values);

            foreach(var explosion in explosions)
                UpdateExplosion(explosion);

        }

        void UpdateBomb(IBomb bomb)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            if (bomb.ShouldExplode(now))
                ExplodeBomb(bomb);
        }

        void UpdateExplosion(Explosion explosion)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            if (explosion.IsExpired(now))
                _explosions.Remove(explosion.Id);
        }

        public bool PlaceBomb(Player player, out IBomb? bomb)
        {
            bomb = null;
            
            if (player.State == PlayerState.Dead)
                return false;

            int activeBombs = _bombs.Values.Count(b => b.OwnerId == player.Id);

            if (activeBombs >= player.BombLimit)
                return false;

            GridPosition position = Map.WorldToGridPosition(player.Position, Player.PlayerSize);

            if (!_map.IsWalkable(position))
                return false;

            bool occupied = _bombs.Values.Any(b => b.Position == position);

            if (occupied)
                return false;

            // Uses the player's composed decorator chain
            bomb = player.CreateDecoratedBomb(Guid.NewGuid(), position);

            _bombs.Add(bomb.Id, bomb);

            return true;
        }

        /// <summary>
        /// Detonates all remote-detonation bombs placed by the specified player.
        /// Uses the IBomb.TryDetonate() polymorphic method on the decorator chain.
        /// </summary>
        public void DetonatePlayerBombs(Guid playerId)
        {
            List<IBomb> bombsToDetonate = _bombs.Values
                .Where(b => b.OwnerId == playerId)
                .ToList();

            foreach (var bomb in bombsToDetonate)
            {
                if (bomb.TryDetonate())
                {
                    // If bomb was detonated, trigger explosion immediately
                    ExplodeBomb(bomb);
                }
            }
        }

        /// <summary>
        /// Places a pre-built (potentially decorated) bomb onto the map.
        /// The client code is responsible for composing decorators before calling this.
        /// </summary>
        public bool PlaceDecoratedBomb(Player player, IBomb decoratedBomb)
        {
            if (player.State == PlayerState.Dead)
                return false;

            int activeBombs = _bombs.Values.Count(b => b.OwnerId == player.Id);

            if (activeBombs >= player.BombLimit)
                return false;

            bool occupied = _bombs.Values.Any(b => b.Position == decoratedBomb.Position);

            if (occupied)
                return false;

            _bombs.Add(decoratedBomb.Id, decoratedBomb);
            return true;
        }

        public bool RemoveBomb(Guid bombId)
        {
            bool found = _bombs.TryGetValue(bombId, out IBomb? bomb);

            if (!found || bomb is null)
                return false;

            _bombs.Remove(bombId);
            return true;
        }

        public List<GridPosition> ExplodeBomb(IBomb bomb)
        {
            List<GameEvent> events = [];
            List<GridPosition> affectedCells;

            _bombs.Remove(bomb.Id);

            // Decorator-aware: uses bomb.IsPiercing to decide wall behavior
            affectedCells = ComputeExplosionCells(bomb.Position, bomb.Radius, bomb.IsPiercing);

            DateTimeOffset occurredAt = DateTimeOffset.UtcNow;

            Explosion explosion = new(
                Guid.NewGuid(),
                bomb.Id,
                bomb.OwnerId,
                bomb.Position,
                affectedCells,
                occurredAt,
                ExplosionDurationSeconds);

            _explosions.Add(explosion.Id, explosion);

            events.Add(
                new BombExplodedEvent(
                    Guid.NewGuid(),
                    occurredAt,
                    bomb.Id,
                    bomb.OwnerId,
                    bomb.Position,
                    [.. affectedCells]));

            foreach (GridPosition cell in affectedCells)
            {
                MapCell mapCell = _map.GetCell(cell);

                if (mapCell is not DestructibleWall)
                    continue;

                Guid wallId = mapCell.Id;

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

            DamagePlayers(bomb, explosion, events);

            foreach (GameEvent gameEvent in events)
                _eventPublisher.Notify(gameEvent);

            return affectedCells;
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

        /// <summary>
        /// Computes which cells are hit by an explosion.
        /// When piercing is true (from PiercingBombDecorator), the explosion
        /// passes through destructible walls instead of stopping.
        /// </summary>
        private List<GridPosition> ComputeExplosionCells(
            GridPosition origin, int radius, bool piercing = false)
        {
            List<GridPosition> cells = [origin];

            (int dx, int dy)[] directions =
            [
                (0, -1),
                (0, 1),
                (-1, 0),
                (1, 0),
            ];

            foreach ((int dx, int dy) in directions)
            {
                for (int step = 1; step <= radius; step++)
                {
                    GridPosition current = new(
                        origin.X + dx * step,
                        origin.Y + dy * step);

                    if (!_map.IsInsideMap(current))
                        break;

                    MapCell cell = _map.GetCell(current);

                    // Indestructible walls always stop the explosion
                    if (cell is IndestructibleWall)
                        break;

                    cells.Add(current);

                    // Piercing bombs go through destructible walls;
                    // normal bombs stop after hitting one
                    if (cell is DestructibleWall && !piercing)
                        break;
                }
            }

            return cells;
        }

        public List<IBomb> GetBombsToExplode(DateTimeOffset now)
        {
            return [.. _bombs.Values.Where(b => b.ShouldExplode(now))];
        }

        public List<IBomb> GetBombs()
        {
            return [.. _bombs.Values];
        }

        public List<Explosion> GetExplosions()
        {
            return [.. _explosions.Values];
        }

        public List<PlaceBombCommand> GetBombCommands()
        {
            return _commands;
        }

        public void AddBombCommand(PlaceBombCommand command)
        {
            _commands.Add(command);
        }
    }
}
