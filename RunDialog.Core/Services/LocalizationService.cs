using RunDialog.Core.Interfaces;
using System.Globalization;

namespace RunDialog.Core.Services;

/// <summary>
/// Localization service with English keys (source language).
/// Values provided for Polish (pl-PL) and English (en-US).
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    private readonly Dictionary<string, Dictionary<string, string>> _resources = new(StringComparer.Ordinal);
    private string _currentCulture = "pl-PL";

    public IReadOnlyList<string> SupportedCultures { get; } = new[] { "pl-PL", "en-US" };

    public string CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (!SupportedCultures.Contains(value))
                throw new CultureNotFoundException($"Culture '{value}' is not supported.");

            _currentCulture = value;
        }
    }

    public LocalizationService()
    {
        LoadResources();
    }

    public string GetString(string key)
    {
        if (_resources.TryGetValue(_currentCulture, out var cultureDict))
        {
            if (cultureDict.TryGetValue(key, out var value))
                return value;
        }

        // Fallback to English value if key exists in en-US
        if (_resources.TryGetValue("en-US", out var enDict) && enDict.TryGetValue(key, out var enValue))
            return enValue;

        return $"[{key}]";
    }

    private void LoadResources()
    {
        _resources["en-US"] = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["WindowTitle"] = "Run",
            ["OpenPrompt"] = "Open:",
            ["DescriptionText"] = "Type the name of a program, folder, document, or Internet resource, and Windows will open it.",
            ["OkButton"] = "OK",
            ["CancelButton"] = "Cancel",
            ["BrowseButton"] = "Browse...",
            ["RunAsAdmin"] = "Run as administrator",
            ["Placeholder"] = "Type a command...",
            ["ErrorNotFound"] = "Windows cannot find '{0}'. Make sure you typed the name correctly, and then try again.",
            ["ErrorNoStrategy"] = "Unable to execute the specified command.",
            ["ErrorInvalidUri"] = "The URL is invalid or cannot be opened.",
            ["ErrorInvalidFolder"] = "The folder path is invalid or cannot be accessed.",
            ["ErrorAccessDenied"] = "Access denied. You do not have permission to run this command.",
            ["ErrorAdminCancelled"] = "The operation was cancelled. Administrator privileges are required.",
            ["HistoryEmpty"] = "No recent commands.",
            ["SettingsTitle"] = "Settings",
            ["OptionsTitle"] = "Options",
            ["ThemeLabel"] = "Theme",
            ["ThemeAuto"] = "Auto",
            ["ThemeLight"] = "Light",
            ["ThemeDark"] = "Dark",
            ["BackdropLabel"] = "Backdrop",
            ["BackdropMica"] = "Mica",
            ["BackdropMicaAlt"] = "Mica Alt",
            ["BackdropAcrylic"] = "Acrylic",
            ["AboutTitle"] = "About",
            ["AboutPlaceholder"] = "Run Dialog v1.0\nA modern open-source Run dialog for Windows."
        };

        _resources["pl-PL"] = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["WindowTitle"] = "Uruchamianie",
            ["OpenPrompt"] = "Otwórz:",
            ["DescriptionText"] = "Wpisz nazwę programu, folderu, dokumentu lub zasobu internetowego, a zostanie on otwarty przez system Windows.",
            ["OkButton"] = "OK",
            ["CancelButton"] = "Anuluj",
            ["BrowseButton"] = "Przeglądaj...",
            ["RunAsAdmin"] = "Uruchom jako administrator",
            ["Placeholder"] = "Wpisz polecenie...",
            ["ErrorNotFound"] = "System Windows nie może odnaleźć '{0}'. Upewnij się, że wpisana nazwa jest poprawna, a następnie spróbuj ponownie.",
            ["ErrorNoStrategy"] = "Nie można wykonać podanego polecenia.",
            ["ErrorInvalidUri"] = "Adres URL jest nieprawidłowy lub nie można go otworzyć.",
            ["ErrorInvalidFolder"] = "Ścieżka folderu jest nieprawidłowa lub nie można uzyskać do niej dostępu.",
            ["ErrorAccessDenied"] = "Odmowa dostępu. Nie masz uprawnień do uruchomienia tego polecenia.",
            ["ErrorAdminCancelled"] = "Operacja została anulowana. Wymagane są uprawnienia administratora.",
            ["HistoryEmpty"] = "Brak ostatnich poleceń.",
            ["SettingsTitle"] = "Ustawienia",
            ["OptionsTitle"] = "Opcje",
            ["ThemeLabel"] = "Motyw",
            ["ThemeAuto"] = "Automatyczny",
            ["ThemeLight"] = "Jasny",
            ["ThemeDark"] = "Ciemny",
            ["BackdropLabel"] = "Tło",
            ["BackdropMica"] = "Mica",
            ["BackdropMicaAlt"] = "Mica Alt",
            ["BackdropAcrylic"] = "Acrylic",
            ["AboutTitle"] = "Informacje",
            ["AboutPlaceholder"] = "Okno Uruchamianie v1.0\nNowoczesne okno Uruchamianie dla Windows."
        };
    }
}
