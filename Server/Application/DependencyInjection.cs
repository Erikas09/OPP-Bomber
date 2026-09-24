using Bomber.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bomber.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<GameStateService>();

        return services;
    }
}
