namespace Bomber.Host.Interpreter.Expressions;

public sealed class SequenceExpression : IConsoleExpression
{
    private readonly List<IConsoleExpression> _expressions;

    public SequenceExpression(IEnumerable<IConsoleExpression> expressions)
    {
        _expressions = [.. expressions];
    }

    public void Interpret(ConsoleContext context)
    {
        foreach (IConsoleExpression expression in _expressions)
            expression.Interpret(context);
    }
}
