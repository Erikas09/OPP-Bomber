using Bomber.Application;
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

        WebApplication app = builder.Build();

        app.MapGet("/", () => "Bomber server is running.");

        app.MapHub<GameHub>("/game");

        app.Run();
    }
}