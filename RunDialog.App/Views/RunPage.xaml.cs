using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RunDialog.App.ViewModels;
using RunDialog.Core.DependencyInjection;
using RunDialog.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Windows.Foundation;

namespace RunDialog.App.Views;

public sealed partial class RunPage : Page
{
    public MainViewModel ViewModel { get; }

    private long _expanderCallbackToken;

    public RunPage()
    {
        var services = new ServiceCollection();
        services.AddRunDialogCore();
        var provider = services.BuildServiceProvider();

        ViewModel = new MainViewModel(
            provider.GetRequiredService<ICommandParser>(),
            provider.GetRequiredService<ICommandExecutor>(),
            provider.GetRequiredService<IHistoryRepository>(),
            provider.GetRequiredService<ILocalizationService>(),
            provider.GetRequiredService<IRunDialogSettings>()
        );

        ViewModel.RequestClose += OnRequestClose;
        ViewModel.RequestBrowse += OnRequestBrowse;
        ViewModel.RequestAbout += OnRequestAbout;
        ViewModel.ThemeChanged += OnThemeChanged;
        ViewModel.BackdropChanged += OnBackdropChanged;

        this.InitializeComponent();

        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;

        CommandInput.QuerySubmitted += OnQuerySubmitted;
        CommandInput.SuggestionChosen += OnSuggestionChosen;

        // Wire up expander events for dynamic window resizing
        _expanderCallbackToken = OptionsExpander.RegisterPropertyChangedCallback(Expander.IsExpandedProperty, (sender, dp) =>
        {
            ResizeWindowToFit();
        });

        // Set custom title bar
        if (App.CurrentWindow != null)
        {
            App.CurrentWindow.ExtendsContentIntoTitleBar = true;
            App.CurrentWindow.SetTitleBar(CustomTitleBar);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ResizeWindowToFit();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.RequestClose -= OnRequestClose;
        ViewModel.RequestBrowse -= OnRequestBrowse;
        ViewModel.RequestAbout -= OnRequestAbout;
        ViewModel.ThemeChanged -= OnThemeChanged;
        ViewModel.BackdropChanged -= OnBackdropChanged;
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;

        CommandInput.QuerySubmitted -= OnQuerySubmitted;
        CommandInput.SuggestionChosen -= OnSuggestionChosen;

        OptionsExpander.UnregisterPropertyChangedCallback(Expander.IsExpandedProperty, _expanderCallbackToken);
    }

    private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion != null)
        {
            ViewModel.CommandText = args.ChosenSuggestion.ToString() ?? string.Empty;
        }
        ViewModel.RunCommand.Execute(null);
    }

    private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem != null)
        {
            ViewModel.CommandText = args.SelectedItem.ToString() ?? string.Empty;
        }
    }

    private void OnRequestClose(object? sender, EventArgs e)
    {
        App.CurrentWindow?.Close();
    }

    private async void OnRequestBrowse(object? sender, EventArgs e)
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
        picker.FileTypeFilter.Add("*");

        if (App.CurrentWindow != null)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        }

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            ViewModel.SetBrowsedPath(file.Path);
        }
    }

    private void OnRequestAbout(object? sender, EventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = ViewModel.AboutTitle,
            Content = new TextBlock { Text = ViewModel.AboutPlaceholder, TextWrapping = TextWrapping.Wrap },
            CloseButtonText = ViewModel.OkButton,
            XamlRoot = this.XamlRoot
        };
        _ = dialog.ShowAsync();
    }

    private void OnThemeChanged(object? sender, string theme)
    {
        if (App.CurrentWindow?.Content is FrameworkElement root)
        {
            root.RequestedTheme = theme switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
        }
    }

    private void OnBackdropChanged(object? sender, string backdrop)
    {
        if (App.CurrentWindow != null)
        {
            App.CurrentWindow.SystemBackdrop = backdrop switch
            {
                "MicaAlt" => new Microsoft.UI.Xaml.Media.MicaBackdrop { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt },
                "Acrylic" => new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop(),
                _ => new Microsoft.UI.Xaml.Media.MicaBackdrop()
            };
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.DescriptionText))
        {
            this.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () => ResizeWindowToFit());
        }
    }

    private void ResizeWindowToFit()
    {
        if (App.CurrentWindow?.AppWindow is not { } appWindow)
            return;

        if (this.ActualWidth <= 0)
            return;

        // Measure with the known client width so text wrapping is computed correctly.
        double measureWidth = this.ActualWidth > 0 ? this.ActualWidth : 560;
        this.Measure(new Size(measureWidth, double.PositiveInfinity));

        // DesiredSize is in DIPs; AppWindow APIs use physical pixels.
        double scale = this.XamlRoot?.RasterizationScale ?? 1.0;
        var desiredHeight = (int)Math.Ceiling(this.DesiredSize.Height * scale);
        var desiredWidth = (int)Math.Ceiling(560 * scale);

        // Ensure minimum collapsed height (400 DIPs converted to physical pixels).
        int minHeightPhys = (int)Math.Ceiling(400 * scale);
        if (desiredHeight < minHeightPhys)
            desiredHeight = minHeightPhys;

        var displayArea = Microsoft.UI.Windowing.DisplayArea.Primary;
        if (displayArea != null)
        {
            var workArea = displayArea.WorkArea;
            // Cap to available work area so the window never grows larger than the monitor.
            if (desiredHeight > workArea.Height)
                desiredHeight = workArea.Height;

            int x = workArea.X;
            int y = workArea.Y + workArea.Height - desiredHeight;
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = desiredWidth, Height = desiredHeight });
            appWindow.Move(new Windows.Graphics.PointInt32 { X = x, Y = y });
        }
        else
        {
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = desiredWidth, Height = desiredHeight });
        }
    }
}
