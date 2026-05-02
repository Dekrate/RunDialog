using Microsoft.UI.Xaml.Data;
using RunDialog.App.Common;
using RunDialog.Core.Interfaces;
using RunDialog.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RunDialog.App.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly ICommandParser _parser;
    private readonly ICommandExecutor _executor;
    private readonly IHistoryRepository _history;
    private readonly ILocalizationService _localization;
    private readonly IRunDialogSettings _settings;

    private string _commandText = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isError;
    private bool _runAsAdmin;
    private string _selectedHistoryItem = string.Empty;
    private string _theme = "Auto";
    private string _backdrop = "Mica";
    private bool _showOptions;

    public string CommandText
    {
        get => _commandText;
        set
        {
            if (SetProperty(ref _commandText, value))
            {
                ((RelayCommand)RunCommand).RaiseCanExecuteChanged();
                FilterHistory();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public bool IsError
    {
        get => _isError;
        private set => SetProperty(ref _isError, value);
    }

    public bool RunAsAdmin
    {
        get => _runAsAdmin;
        set
        {
            if (SetProperty(ref _runAsAdmin, value))
            {
                _settings.RunAsAdministrator = value;
            }
        }
    }

    public string SelectedHistoryItem
    {
        get => _selectedHistoryItem;
        set
        {
            if (SetProperty(ref _selectedHistoryItem, value))
            {
                CommandText = value;
            }
        }
    }

    public string Theme
    {
        get => _theme;
        set
        {
            if (SetProperty(ref _theme, value))
            {
                _settings.Theme = value;
                ThemeChanged?.Invoke(this, value);
            }
        }
    }

    public string Backdrop
    {
        get => _backdrop;
        set
        {
            if (SetProperty(ref _backdrop, value))
            {
                _settings.Backdrop = value;
                BackdropChanged?.Invoke(this, value);
            }
        }
    }

    public bool ShowOptions
    {
        get => _showOptions;
        set => SetProperty(ref _showOptions, value);
    }

    public ObservableCollection<string> HistoryItems { get; } = new();
    public ObservableCollection<string> FilteredHistory { get; } = new();

    public ICommand RunCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand BrowseCommand { get; }
    public ICommand ToggleLanguageCommand { get; }
    public ICommand ToggleOptionsCommand { get; }
    public ICommand ShowAboutCommand { get; }

    public string WindowTitle => _localization.GetString("WindowTitle");
    public string OpenPrompt => _localization.GetString("OpenPrompt");
    public string DescriptionText => _localization.GetString("DescriptionText");
    public string OkButton => _localization.GetString("OkButton");
    public string CancelButton => _localization.GetString("CancelButton");
    public string BrowseButton => _localization.GetString("BrowseButton");
    public string RunAsAdminText => _localization.GetString("RunAsAdmin");
    public string PlaceholderText => _localization.GetString("Placeholder");
    public string OptionsTitle => _localization.GetString("OptionsTitle");
    public string ThemeLabel => _localization.GetString("ThemeLabel");
    public string ThemeAuto => _localization.GetString("ThemeAuto");
    public string ThemeLight => _localization.GetString("ThemeLight");
    public string ThemeDark => _localization.GetString("ThemeDark");
    public string BackdropLabel => _localization.GetString("BackdropLabel");
    public string BackdropMica => _localization.GetString("BackdropMica");
    public string BackdropMicaAlt => _localization.GetString("BackdropMicaAlt");
    public string BackdropAcrylic => _localization.GetString("BackdropAcrylic");
    public string AboutTitle => _localization.GetString("AboutTitle");
    public string AboutPlaceholder => _localization.GetString("AboutPlaceholder");

    public MainViewModel(
        ICommandParser parser,
        ICommandExecutor executor,
        IHistoryRepository history,
        ILocalizationService localization,
        IRunDialogSettings settings)
    {
        _parser = parser;
        _executor = executor;
        _history = history;
        _localization = localization;
        _settings = settings;
        _runAsAdmin = settings.RunAsAdministrator;
        _theme = settings.Theme;
        _backdrop = settings.Backdrop;

        RunCommand = new RelayCommand(_ => ExecuteRun(), _ => !string.IsNullOrWhiteSpace(CommandText));
        CancelCommand = new RelayCommand(_ => ExecuteCancel());
        BrowseCommand = new RelayCommand(_ => ExecuteBrowse());
        ToggleLanguageCommand = new RelayCommand(_ => ToggleLanguage());
        ToggleOptionsCommand = new RelayCommand(_ => ShowOptions = !ShowOptions);
        ShowAboutCommand = new RelayCommand(_ => RequestAbout?.Invoke(this, EventArgs.Empty));

        LoadHistory();
    }

    private void ExecuteRun()
    {
        try
        {
            IsError = false;
            StatusMessage = string.Empty;

            var command = _parser.Parse(CommandText);
            var result = _executor.Execute(command);

            if (result.Success)
            {
                _history.Add(CommandText);
                LoadHistory();
                StatusMessage = $"✓ {result.ExecutedPath}";
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                IsError = true;
                StatusMessage = result.ErrorMessage ?? _localization.GetString("ErrorNoStrategy");
            }
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = ex.Message;
        }
    }

    private void ExecuteCancel()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private void ExecuteBrowse()
    {
        RequestBrowse?.Invoke(this, EventArgs.Empty);
    }

    private void ToggleLanguage()
    {
        var current = _localization.CurrentCulture;
        var next = current == "pl-PL" ? "en-US" : "pl-PL";
        _localization.CurrentCulture = next;

        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(OpenPrompt));
        OnPropertyChanged(nameof(DescriptionText));
        OnPropertyChanged(nameof(OkButton));
        OnPropertyChanged(nameof(CancelButton));
        OnPropertyChanged(nameof(BrowseButton));
        OnPropertyChanged(nameof(RunAsAdminText));
        OnPropertyChanged(nameof(PlaceholderText));
        OnPropertyChanged(nameof(OptionsTitle));
        OnPropertyChanged(nameof(ThemeLabel));
        OnPropertyChanged(nameof(ThemeAuto));
        OnPropertyChanged(nameof(ThemeLight));
        OnPropertyChanged(nameof(ThemeDark));
        OnPropertyChanged(nameof(BackdropLabel));
        OnPropertyChanged(nameof(BackdropMica));
        OnPropertyChanged(nameof(BackdropMicaAlt));
        OnPropertyChanged(nameof(BackdropAcrylic));
        OnPropertyChanged(nameof(AboutTitle));
        OnPropertyChanged(nameof(AboutPlaceholder));
    }

    public void SetBrowsedPath(string path)
    {
        CommandText = path;
    }

    private void LoadHistory()
    {
        HistoryItems.Clear();
        foreach (var item in _history.GetHistory())
        {
            HistoryItems.Add(item);
        }
        FilterHistory();
    }

    private void FilterHistory()
    {
        FilteredHistory.Clear();
        var text = CommandText?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(text))
        {
            foreach (var item in HistoryItems.Take(5))
                FilteredHistory.Add(item);
            return;
        }

        var matches = HistoryItems
            .Where(h => h.Contains(text, StringComparison.OrdinalIgnoreCase))
            .Take(5);

        foreach (var item in matches)
            FilteredHistory.Add(item);
    }

    public event EventHandler? RequestClose;
    public event EventHandler? RequestBrowse;
    public event EventHandler? RequestAbout;
    public event EventHandler<string>? ThemeChanged;
    public event EventHandler<string>? BackdropChanged;
}
