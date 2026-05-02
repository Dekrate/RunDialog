namespace RunDialog.Core.Interfaces;

/// <summary>
/// Localization service with keys in English (design-time / source language).
/// Supports Polish and English at runtime.
/// </summary>
public interface ILocalizationService
{
    string GetString(string key);
    string CurrentCulture { get; set; }
    IReadOnlyList<string> SupportedCultures { get; }
}
