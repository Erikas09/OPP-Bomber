using Bomber.Domain.Enums;
using Bomber.Host.Interpreter.Expressions;
using System.Globalization;
using System.Linq.Expressions;

namespace Bomber.Host.Interpreter;

public sealed class ConsoleInterpreter
{
    public IConsoleExpression Parse(string input)
    {
        string[] commands = input.Split(';', StringSplitOptions.TrimEntries);
        List<IConsoleExpression> expressions = [];

        foreach (string command in commands)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new FormatException("Empty command. Type 'help' for available commands.");

            expressions.Add(ParseCommand(command));
        }

        if (expressions.Count == 1)
            return expressions[0];

        return new SequenceExpression(expressions);
    }

    private IConsoleExpression ParseCommand(string command)
    {
        string[] tokens = command.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        return tokens[0].ToLowerInvariant() switch
        {
            "game" => new GameExpression(ParseGame(tokens)),
            "bot" => new BotExpression(ParseBot(tokens)),
            "help" when tokens.Length == 1 => new HelpExpression(),
            _ => throw new FormatException("Unknown command. Type 'help' for available commands."),
        };
    }

    private IConsoleExpression ParseGame(string[] tokens)
    {
        if (tokens.Length != 2)
            throw new FormatException("Usage: game pause|resume|reset|status");

        return tokens[1].ToLowerInvariant() switch
        {
            "pause" => new PauseExpression(),
            "resume" => new ResumeExpression(),
            "reset" => new ResetExpression(),
            "status" => new StatusExpression(),
            _ => throw new FormatException("Unknown game action."),
        };
    }

    private IConsoleExpression ParseBot(string[] tokens)
    {
        if (tokens.Length < 2)
            throw new FormatException("Usage: bot add|remove|list");

        return tokens[1].ToLowerInvariant() switch
        {
            "add" => ParseAddBot(tokens),
            "remove" => ParseRemoveBot(tokens),
            "list" when tokens.Length == 2 => new ListBotsExpression(),
            _ => throw new FormatException("Unknown bot action or invalid arguments."),
        };
    }

    private IConsoleExpression ParseAddBot(string[] tokens)
    {
        if (tokens.Length != 3 && tokens.Length != 4)
            throw new FormatException("Usage: bot add hunter|defender [count]");

        BotType type = ParseBotType(tokens[2]);
        int count = 1;

        if (tokens.Length == 4 &&
            (!int.TryParse(tokens[3], NumberStyles.None, CultureInfo.InvariantCulture, out count) || count <= 0))
            throw new FormatException("Bot count must be a positive integer.");

        return new AddBotExpression(type, count);
    }

    private IConsoleExpression ParseRemoveBot(string[] tokens)
    {
        if (tokens.Length != 3)
            throw new FormatException("Usage: bot remove all|hunter|defender|<id>");

        string target = tokens[2].ToLowerInvariant();

        if (target == "all")
            return new RemoveBotExpression();

        if (target == "hunter" || target == "defender")
            return new RemoveBotExpression(type: ParseBotType(target));

        if (Guid.TryParse(target, out Guid id))
            return new RemoveBotExpression(id: id);

        throw new FormatException("Invalid bot target. Use 'bot list' to find bot IDs.");
    }

    private BotType ParseBotType(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "hunter" => BotType.Hunter,
            "defender" => BotType.Defender,
            _ => throw new FormatException("Unknown bot type. Use hunter or defender."),
        };
    }
}
