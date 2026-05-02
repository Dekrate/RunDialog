using RunDialog.Core.Interfaces;
using RunDialog.Core.Models;
using System.ComponentModel;
using System.Diagnostics;

namespace RunDialog.Core.Strategies;

/// <summary>
/// Strategy for executing programs, executables, and system commands.
/// Supports elevated execution when RunAsAdministrator is enabled.
/// </summary>
public sealed class ProgramExecutionStrategy : IExecutionStrategy
{
    private readonly ILocalizationService _localization;
    private readonly IRunDialogSettings _settings;

    public ProgramExecutionStrategy(ILocalizationService localization, IRunDialogSettings settings)
    {
        _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public bool CanExecute(RunCommand command)
    {
        return command.Type == CommandType.Program || command.Type == CommandType.Unknown;
    }

    public ExecutionResult Execute(RunCommand command)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command.RawInput,
                UseShellExecute = true,
                Verb = _settings.RunAsAdministrator ? "runas" : string.Empty
            };

            if (!string.IsNullOrEmpty(command.Arguments))
            {
                startInfo.Arguments = command.Arguments;
                startInfo.FileName = command.RawInput.Split(' ')[0];
            }

            Process.Start(startInfo);
            return ExecutionResult.Ok(startInfo.FileName);
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
        {
            // 1223 = ERROR_CANCELLED (user declined UAC prompt)
            return ExecutionResult.Fail(_localization.GetString("ErrorAdminCancelled"));
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 740)
        {
            // 740 = ERROR_ELEVATION_REQUIRED (needs admin but not requested)
            return ExecutionResult.Fail(_localization.GetString("ErrorAccessDenied"));
        }
        catch (Exception ex)
        {
            var message = string.Format(_localization.GetString("ErrorNotFound"), command.RawInput);
            return ExecutionResult.Fail(message + $" ({ex.Message})");
        }
    }
}
