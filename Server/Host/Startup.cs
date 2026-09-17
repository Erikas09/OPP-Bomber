using Bomber.Application.Services;
using Bomber.Host.Hubs;

namespace Bomber.Host;

internal static class Startup
{
    internal static void Main(params string[] arguments)
    {
        var builder = WebApplication.CreateBuilder(arguments);

        builder.Services.AddSignalR();

        builder.Services.AddSingleton<GameStateService>();

        WebApplication app = builder.Build();

        app.MapHub<GameHub>("/game");

        app.Run();
    }
}