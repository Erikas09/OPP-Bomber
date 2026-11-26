namespace Bomber.Host.Interpreter.Expressions;

public sealed class ResumeExpression : IConsoleExpression
{
    public void Interpret(ConsoleContext context)
    {
        context.Output.WriteLine(context.Administration.Resume());
    }
}
