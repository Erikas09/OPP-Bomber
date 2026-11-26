namespace Bomber.Host.Interpreter.Expressions;

public sealed class ListBotsExpression : IConsoleExpression
{
    public void Interpret(ConsoleContext context)
    {
        context.Output.WriteLine(context.Administration.ListBots());
    }
}
