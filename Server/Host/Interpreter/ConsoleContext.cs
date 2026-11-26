using Bomber.Application.Services;

namespace Bomber.Host.Interpreter;

public sealed class ConsoleContext
{
    public GameAdministrationService Administration { get; }
    public TextWriter Output { get; }

    public ConsoleContext(GameAdministrationService administration, TextWriter output)
    {
        Administration = administration;
        Output = output;
    }
}
