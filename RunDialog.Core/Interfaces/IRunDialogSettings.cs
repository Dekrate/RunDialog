namespace RunDialog.Core.Interfaces;

public interface IRunDialogSettings
{
    bool ShowAutoComplete { get; }
    int MaxHistoryItems { get; }
    bool RunAsAdministrator { get; set; }
    string Theme { get; set; }           // Auto, Light, Dark
    string Backdrop { get; set; }        // Mica, MicaAlt, Acrylic
}
