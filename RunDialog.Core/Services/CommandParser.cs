using RunDialog.Core.Interfaces;
using RunDialog.Core.Models;

namespace RunDialog.Core.Services;

/// <summary>
/// Single Responsibility: parses raw input and classifies command type.
/// </summary>
public sealed class CommandParser : ICommandParser
{
    public RunCommand Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be empty.", nameof(input));

        var trimmed = input.Trim();
        var type = DetectType(trimmed);
        var (path, args) = SplitPathAndArguments(trimmed, type);

        return new RunCommand(trimmed, type, args);
    }

    private static CommandType DetectType(string input)
    {
        if (IsUri(input))
            return CommandType.Uri;

        if (IsFolderPath(input))
            return CommandType.Folder;

        if (IsKnownProgram(input))
            return CommandType.Program;

        return CommandType.Unknown;
    }

    private static bool IsUri(string input)
    {
        return input.Contains("://", StringComparison.OrdinalIgnoreCase)
            || input.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || input.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || input.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase)
            || input.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsFolderPath(string input)
    {
        return Path.IsPathRooted(input)
            || input.StartsWith("\\\\", StringComparison.Ordinal);
    }

    private static bool IsKnownProgram(string input)
    {
        // Allow any executable-like input; executor will validate existence.
        return true;
    }

    private static (string Path, string? Args) SplitPathAndArguments(string input, CommandType type)
    {
        if (type == CommandType.Uri)
            return (input, null);

        var firstSpace = input.IndexOf(' ');
        if (firstSpace < 0)
            return (input, null);

        return (input[..firstSpace], input[(firstSpace + 1)..]);
    }
}
