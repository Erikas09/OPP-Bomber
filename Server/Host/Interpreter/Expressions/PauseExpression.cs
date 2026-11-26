namespace Bomber.Host.Interpreter.Expressions;

public sealed class PauseExpression : IConsoleExpression
{
    public void Interpret(ConsoleContext context)
    {
        context.Output.WriteLine(context.Administration.Pause());
    }
}
