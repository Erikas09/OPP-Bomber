using Bomber.Domain.Enums;

namespace Bomber.Host.Interpreter.Expressions;

public sealed class RemoveBotExpression : IConsoleExpression
{
    private readonly Guid? _id;
    private readonly BotType? _type;

    public RemoveBotExpression(Guid? id = null, BotType? type = null)
    {
        _id = id;
        _type = type;
    }

    public void Interpret(ConsoleContext context)
    {
        context.Output.WriteLine(context.Administration.RemoveBots(_id, _type));
    }
}
