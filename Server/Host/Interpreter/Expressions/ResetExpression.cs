namespace Bomber.Host.Interpreter.Expressions;

public sealed class ResetExpression : IConsoleExpression
{
    public void Interpret(ConsoleContext context)
    {
        context.Output.WriteLine(context.Administration.Reset());
    }
}
