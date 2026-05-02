using RunDialog.Core.Interfaces;
using RunDialog.Core.Models;
using System.Diagnostics;

namespace RunDialog.Core.Strategies;

/// <summary>
/// Strategy for opening URIs and web links.
/// </summary>
public sealed class UriExecutionStrategy : IExecutionStrategy
{
    private readonly ILocalizationService _localization;

    public UriExecutionStrategy(ILocalizationService localization)
    {
        _localization = localization ?? throw new ArgumentNullException(nameof(localization));
    }

    public bool CanExecute(RunCommand command)
    {
        return command.Type == CommandType.Uri;
    }

    public ExecutionResult Execute(RunCommand command)
    {
        try
        {
            var uri = new Uri(command.RawInput);
            var startInfo = new ProcessStartInfo
            {
                FileName = uri.ToString(),
                UseShellExecute = true
            };

            Process.Start(startInfo);
            return ExecutionResult.Ok(uri.ToString());
        }
        catch (UriFormatException)
        {
            return ExecutionResult.Fail(_localization.GetString("ErrorInvalidUri"));
        }
        catch (Exception ex)
        {
            return ExecutionResult.Fail(_localization.GetString("ErrorInvalidUri") + $" ({ex.Message})");
        }
    }
}
