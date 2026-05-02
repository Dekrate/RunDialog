using RunDialog.Core.Interfaces;
using RunDialog.Core.Models;
using System.Diagnostics;

namespace RunDialog.Core.Strategies;

/// <summary>
/// Strategy for opening local and network folders.
/// </summary>
public sealed class FolderExecutionStrategy : IExecutionStrategy
{
    private readonly ILocalizationService _localization;

    public FolderExecutionStrategy(ILocalizationService localization)
    {
        _localization = localization ?? throw new ArgumentNullException(nameof(localization));
    }

    public bool CanExecute(RunCommand command)
    {
        return command.Type == CommandType.Folder;
    }

    public ExecutionResult Execute(RunCommand command)
    {
        try
        {
            var path = command.RawInput;
            if (!Directory.Exists(path))
            {
                // Try to create if user wants (optional); for Run dialog we just open explorer.
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{path}\"",
                UseShellExecute = false
            };

            Process.Start(startInfo);
            return ExecutionResult.Ok(path);
        }
        catch (Exception ex)
        {
            return ExecutionResult.Fail(_localization.GetString("ErrorInvalidFolder") + $" ({ex.Message})");
        }
    }
}
