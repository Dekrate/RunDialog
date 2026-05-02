namespace RunDialog.Core.Services;

public sealed class RunDialogSettings : Interfaces.IRunDialogSettings
{
    public bool ShowAutoComplete { get; init; } = true;
    public int MaxHistoryItems { get; init; } = 25;
    public bool RunAsAdministrator { get; set; } = false;
    public string Theme { get; set; } = "Auto";
    public string Backdrop { get; set; } = "Mica";
}
