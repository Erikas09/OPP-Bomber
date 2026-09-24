using Bomber.Application;
using Bomber.Application.Services;
using Bomber.Host.Hubs;
using Bomber.Host.Services;

namespace Bomber.Host;

internal static class Startup
{
    internal static void Main(params string[] arguments)
    {
        var builder = WebApplication.CreateBuilder(arguments);

        builder.Services.AddSignalR();

        builder.Services.AddApplication();

        builder.Services.AddSingleton<ConnectionRegistry>();
        builder.Services.AddSingleton<GameStateBroadcaster>();
        builder.Services.AddSingleton<GameLoopRunner>();

        WebApplication app = builder.Build();

        app.MapGet("/", () => "Bomber server is running.");

        app.MapHub<GameHub>("/game");

        GameLoopRunner gameLoop =
            app.Services.GetRequiredService<GameLoopRunner>();

        using CancellationTokenSource cancellation =
            new();

        Task gameLoopTask = Task.Run(
            () => gameLoop.Run(cancellation.Token));

        try
        {
            app.Run();
        }
        finally
        {
            cancellation.Cancel();

            gameLoopTask.GetAwaiter().GetResult();
        }
    }
}