using Bomber.Application.Services;
using Bomber.Host.Interpreter.Expressions;
using Microsoft.Extensions.Hosting;

namespace Bomber.Host.Interpreter;

public sealed class GameConsoleRunner : BackgroundService
{
    private readonly ConsoleInterpreter _interpreter;
    private readonly ConsoleContext _context;

    public GameConsoleRunner(ConsoleInterpreter interpreter, GameAdministrationService administration)
    {
        _interpreter = interpreter;
        _context = new ConsoleContext(administration, Console.Out);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Game administration console. Type 'help' for available commands.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.Write("> ");

                // Console.ReadLine may block on Windows; cancel the wait during host shutdown.
                string? input = await Task.Run(() => Console.ReadLine()).WaitAsync(stoppingToken);

                if (input is null)
                    return;

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                IConsoleExpression expression = _interpreter.Parse(input);
                expression.Interpret(_context);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }
        }
    }
}
