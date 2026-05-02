using RunDialog.Core.Models;

namespace RunDialog.Core.Interfaces;

/// <summary>
/// High-level orchestrator that delegates execution to appropriate strategy.
/// Dependency Inversion: depends on abstractions (IExecutionStrategy), not concrete implementations.
/// </summary>
public interface ICommandExecutor
{
    ExecutionResult Execute(RunCommand command);
}
