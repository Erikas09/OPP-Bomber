namespace Bomber.Host.Interpreter.Expressions;

public sealed class GameExpression : IConsoleExpression
{
    private readonly IConsoleExpression _action;

    public GameExpression(IConsoleExpression action)
    {
        _action = action;
    }

    public void Interpret(ConsoleContext context)
    {
        _action.Interpret(context);
    }
}
