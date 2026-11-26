namespace Bomber.Host.Interpreter.Expressions;

public sealed class HelpExpression : IConsoleExpression
{
    public void Interpret(ConsoleContext context)
    {
        context.Output.WriteLine("Available commands:");
        context.Output.WriteLine("  game pause | resume | reset | status");
        context.Output.WriteLine("  bot add hunter|defender [count]");
        context.Output.WriteLine("  bot remove all|hunter|defender|<id>");
        context.Output.WriteLine("  bot list");
        context.Output.WriteLine("  help");
        context.Output.WriteLine("Separate commands with ';' to execute them in sequence.");
    }
}
