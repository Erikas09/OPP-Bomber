namespace Bomber.Host.Interpreter.Expressions;

public sealed class BotExpression : IConsoleExpression
{
    private readonly IConsoleExpression _action;

    public BotExpression(IConsoleExpression action)
    {
        _action = action;
    }

    public void Interpret(ConsoleContext context)
    {
        _action.Interpret(context);
    }
}
