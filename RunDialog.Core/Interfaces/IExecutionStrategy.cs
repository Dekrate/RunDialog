using RunDialog.Core.Models;

namespace RunDialog.Core.Interfaces;

/// <summary>
/// Strategy Pattern: defines algorithm for executing a specific command type.
/// Open/Closed Principle: new command types are added by implementing this interface.
/// </summary>
public interface IExecutionStrategy
{
    bool CanExecute(RunCommand command);
    ExecutionResult Execute(RunCommand command);
}
