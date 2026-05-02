namespace RunDialog.Core.Models;

public sealed class RunCommand
{
    public string RawInput { get; }
    public CommandType Type { get; }
    public string? Arguments { get; }

    public RunCommand(string rawInput, CommandType type, string? arguments = null)
    {
        RawInput = rawInput ?? throw new ArgumentNullException(nameof(rawInput));
        Type = type;
        Arguments = arguments;
    }

    public override string ToString() => RawInput;
}
