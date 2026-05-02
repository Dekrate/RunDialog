using RunDialog.Core.Models;

namespace RunDialog.Core.Interfaces;

/// <summary>
/// Single Responsibility Principle: only parses raw user input into structured command.
/// </summary>
public interface ICommandParser
{
    RunCommand Parse(string input);
}
