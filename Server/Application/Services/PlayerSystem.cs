using Bomber.Domain.Common;
using Bomber.Domain.Entities.Bots;
using Bomber.Domain.Enums;
using Bomber.Domain.Maps;
using Bomber.Domain.Maps.Cells;
using Bomber.Domain.Players;
using Bomber.Domain.PowerUps;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Application.Services
{
    public sealed class PlayerSystem
    {
        private readonly Dictionary<Guid, Player> _players = [];
        readonly Map _map;

        public Dictionary<Guid, Player> AllPlayers => _players;
        public int PlayerCount => _players.Count;

        public PlayerSystem(Map map)
        {
            _map = map;
        }

        public void SpawnBots(List<SpawnPoint> spawnPoints)
        {
            for(int i=0; i < spawnPoints.Count; i++)
            {
                Position position = Map.GridToWorldPosition(spawnPoints[i].Position, Player.PlayerSize);

                Bot bot;

                if (i == 0)
                    bot = BotManager.Instance.CreateHunter(position);
                else
                    bot = BotManager.Instance.CreateDefender(position);

                AddPlayer(bot);
            }
        }

        public void AddPlayer(Guid playerId, string playerName, SpawnPoint spawnPoint)
        {
            if (_players.ContainsKey(playerId))
                return;

            Position position = Map.GridToWorldPosition(spawnPoint.Position, Player.PlayerSize);

            Player player = new(
                playerId,
                playerName,
                position,
                lives: 3,
                bombLimit: 1,
                explosionRadius: 1,
                speed: 120f);

            _players.Add(playerId, player);
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player.Id, player);
        }

        public void RemovePlayer(Guid playerId)
        {
            _players.Remove(playerId);
        }

        public void SetPlayerInput(Guid playerId, PlayerInput input)
        {
            if (_players.TryGetValue(playerId, out Player? value))
                value.SetInput(input);
        }

        public List<Player> GetPlayers()
        {
            return [.. _players.Values];
        }

        public void Update(float deltaTime)
        {
            foreach (Player player in _players.Values)
            {
                if (player.State == PlayerState.Dead)
                    continue;

                UpdatePlayer(player, player.Input, deltaTime);
            }
        }

        private void UpdatePlayer(
    Player player,
    PlayerInput input,
    float deltaTime)
        {
            float movementX = 0f;
            float movementY = 0f;

            if (input.Up)
                movementY -= 1f;

            if (input.Down)
                movementY += 1f;

            if (input.Left)
                movementX -= 1f;

            if (input.Right)
                movementX += 1f;

            float length = MathF.Sqrt(
                movementX * movementX +
                movementY * movementY);

            if (length > 0f)
            {
                movementX /= length;
                movementY /= length;
            }

            float walkingSpeedModifier = 1;
            GridPosition playerPos = Map.WorldToGridPosition(player.Position, Player.PlayerSize);
            MapCell cell = _map.GetCell(playerPos);

            if (cell is ClassicFloor floor)
            {
                walkingSpeedModifier = floor.walkingSpeedModifier;
                float distance = player.Speed * deltaTime * walkingSpeedModifier;

                movementX *= distance;
                movementY *= distance;
            }
            else if (cell is IceFloor ifloor)
            {
                walkingSpeedModifier = ifloor.walkingSpeedModifier;
                float targetVelocityX = movementX * player.Speed * walkingSpeedModifier;
                float targetVelocityY = movementY * player.Speed * walkingSpeedModifier;

                float maxDelta = 2 * deltaTime;
                player.VelocityX = MoveToward(player.VelocityX, targetVelocityX, maxDelta * player.Speed);
                player.VelocityY = MoveToward(player.VelocityY, targetVelocityY, maxDelta * player.Speed);

                movementX = player.VelocityX * deltaTime;
                movementY = player.VelocityY * deltaTime;
            }

            Position horizontalPosition = new(
                player.Position.X + movementX,
                player.Position.Y);

            if (_map.IsWalkable(horizontalPosition, Player.PlayerSize))
            {
                player.MoveTo(horizontalPosition);
            }
            else
                player.VelocityX = 0f;

            Position verticalPosition = new(
                player.Position.X,
                player.Position.Y + movementY);

            if (_map.IsWalkable(verticalPosition, Player.PlayerSize))
            {
                player.MoveTo(verticalPosition);
            }
            else
                player.VelocityY = 0f;

            PowerUp? powerUp = _map.GetPowerUpByPosition(playerPos);

            if (powerUp is not null)
            {
                player.PickUpPowerUp(powerUp);
                _map.RemovePowerUp(playerPos);

                // 25% chance to spawn new powerup in random location
                if (Random.Shared.NextDouble() < 0.25)
                {
                    PowerUpPrototype clonedPowerUp = powerUp.Clone();
                    if (clonedPowerUp is PowerUp castedPowerUp)
                    {
                        GridPosition pos = _map.GetRandomValidPosition();
                        castedPowerUp.Move(pos);
                        _map.AddPowerUp(castedPowerUp);
                    }

                }
            }
        }

        static float MoveToward(float current, float target, float maxDelta)
        {
            float diff = target - current;
            if (MathF.Abs(diff) <= maxDelta)
                return target;
            return current + MathF.Sign(diff) * maxDelta;
        }
    }
}
