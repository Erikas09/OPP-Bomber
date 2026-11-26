using Bomber.Domain.Enums;

namespace Bomber.Host.Interpreter.Expressions;

public sealed class AddBotExpression : IConsoleExpression
{
    private readonly BotType _type;
    private readonly int _count;

    public AddBotExpression(BotType type, int count)
    {
        _type = type;
        _count = count;
    }

    public void Interpret(ConsoleContext context)
    {
        context.Output.WriteLine(context.Administration.AddBots(_type, _count));
    }
}
