using Bomber.Application.Services;
using Bomber.Domain.Game;
using Bomber.Host.Services;
using System.Diagnostics;

public sealed class GameLoopRunner
{
    private const double TickDuration = 1.0 / 60.0;
    private const double SnapshotInterval = 1.0 / 20.0;

    private readonly GameWorld _world;
    private readonly GameFacade _game;
    private readonly GameStateBroadcaster _broadcaster;

    public GameLoopRunner(GameWorld world, GameStateBroadcaster broadcaster, GameFacade game)
    {
        _world = world;
        _broadcaster = broadcaster;
        _game = game;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        double previous = stopwatch.Elapsed.TotalSeconds;

        double accumulator = 0;
        double snapshotAccumulator = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            double current = stopwatch.Elapsed.TotalSeconds;
            double elapsed = current - previous;

            previous = current;

            elapsed = Math.Min(elapsed, 0.25);

            accumulator += elapsed;
            snapshotAccumulator += elapsed;

            while (accumulator >= TickDuration)
            {
                lock (_game.SyncRoot)
                {
                    if (!_game.IsPaused)
                        _world.Update((float)TickDuration);
                }

                accumulator -= TickDuration;
            }

            if (snapshotAccumulator >= SnapshotInterval)
            {
                snapshotAccumulator -= SnapshotInterval;

                await _broadcaster.BroadcastAsync(cancellationToken);
            }

            await Task.Yield();
        }
    }
}
