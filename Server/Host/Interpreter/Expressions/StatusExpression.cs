namespace Bomber.Host.Interpreter.Expressions;

public sealed class StatusExpression : IConsoleExpression
{
    public void Interpret(ConsoleContext context)
    {
        context.Output.WriteLine(context.Administration.Status());
    }
}
