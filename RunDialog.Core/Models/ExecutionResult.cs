namespace RunDialog.Core.Models;

public sealed class ExecutionResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }
    public string? ExecutedPath { get; }

    private ExecutionResult(bool success, string? errorMessage = null, string? executedPath = null)
    {
        Success = success;
        ErrorMessage = errorMessage;
        ExecutedPath = executedPath;
    }

    public static ExecutionResult Ok(string? executedPath = null) => new(true, executedPath: executedPath);
    public static ExecutionResult Fail(string errorMessage) => new(false, errorMessage: errorMessage);
}
