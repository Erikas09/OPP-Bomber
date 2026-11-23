using Bomber.Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Bomber.Client.Rendering.Animations
{
    public class AnimationManager
    {
        private const double TickDuration = 1.0 / 60.0;

        Dictionary<Guid, PlayerAnimator> _animators = new();

        public AnimationManager()
        {
            CancellationTokenSource _cancelTokenSource = new();
            Task gameLoopTask = Task.Run(() => Run(_cancelTokenSource.Token));
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            double previous = stopwatch.Elapsed.TotalSeconds;

            double accumulator = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                double current = stopwatch.Elapsed.TotalSeconds;
                double elapsed = current - previous;

                previous = current;

                elapsed = Math.Min(elapsed, 0.25);

                accumulator += elapsed;

                while (accumulator >= TickDuration)
                {
                    foreach (var animator in _animators.Values)
                        animator.Update((float)TickDuration);

                    accumulator -= TickDuration;
                }

                await Task.Yield();
            }
        }

        public void UpdateData(List<PlayerStateResponse> players)
        {
            var oldPlayers = _animators.Keys;

            foreach(var player in oldPlayers)
            {
                if (!players.Where(p => p.Id == player).Any())
                    _animators.Remove(player);
            }

            foreach(var player in players)
            {
                if (!_animators.ContainsKey(player.Id))
                    _animators.Add(player.Id, new PlayerAnimator(player));

                _animators[player.Id].UpdatePlayerData(player);
            }
        }

        public string GetSheetName(Guid id)
        {
            return _animators[id].GetSheetName();
        }

        public int GetSpriteSheetRow(Guid id)
        {
            return _animators[id].GetSpriteSheetRow();
        }

        public int GetSpriteSheetColumn(Guid id)
        {
            return _animators[id].GetSpriteSheetColumn();
        }
    }
}
