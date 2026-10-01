using Bomber.Application.Observers;
using Bomber.Application.Publishers;
using Bomber.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bomber.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<GameLogObserver>();

        services.AddSingleton<PlayerDeathObserver>();

        services.AddSingleton<WallDestroyedObserver>();

        services.AddSingleton<GameEventPublisher>(
            serviceProvider =>
            {
                GameEventPublisher publisher = new();

                GameLogObserver gameLogObserver =
                    serviceProvider
                        .GetRequiredService<GameLogObserver>();

                PlayerDeathObserver playerDeathObserver =
                    serviceProvider
                        .GetRequiredService<PlayerDeathObserver>();

                WallDestroyedObserver wallDestroyedObserver =
                    serviceProvider
                        .GetRequiredService<WallDestroyedObserver>();

                publisher.Subscribe(gameLogObserver);

                publisher.Subscribe(playerDeathObserver);

                publisher.Subscribe(wallDestroyedObserver);

                return publisher;
            });

        services.AddSingleton<GameStateService>();

        return services;
    }
}
