using RunDialog.Core.Interfaces;
using RunDialog.Core.Models;

namespace RunDialog.Core.Services;

/// <summary>
/// Orchestrator that selects appropriate Strategy based on command type.
/// Open/Closed: new behaviors added via new IExecutionStrategy implementations.
/// Dependency Inversion: depends on IEnumerable<IExecutionStrategy> abstraction.
/// </summary>
public sealed class CommandExecutor : ICommandExecutor
{
    private readonly IEnumerable<IExecutionStrategy> _strategies;

    public CommandExecutor(IEnumerable<IExecutionStrategy> strategies)
    {
        _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
    }

    public ExecutionResult Execute(RunCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var strategy = _strategies.FirstOrDefault(s => s.CanExecute(command));

        if (strategy == null)
        {
            return ExecutionResult.Fail($"No execution strategy found for command type: {command.Type}");
        }

        return strategy.Execute(command);
    }
}
