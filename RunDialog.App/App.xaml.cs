using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using RunDialog.App.Converters;
using RunDialog.App.Views;

namespace RunDialog.App;

public partial class App : Application
{
    private Window? _window;
    public static Window? CurrentWindow { get; private set; }

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new Window
        {
            Title = "Run",
            SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop(),
            Content = new Frame()
        };
        CurrentWindow = _window;

        // Configure presenter: fixed size dialog, non-resizable, no maximize/snap
        var presenter = Microsoft.UI.Windowing.OverlappedPresenter.Create();
        presenter.IsMaximizable = false;
        presenter.IsResizable = false;
        presenter.IsMinimizable = true;
        _window.AppWindow.SetPresenter(presenter);

        // Apply custom sizing for a Run dialog appearance
        _window.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 560, Height = 340 });
        // Position in the bottom-left corner of the primary monitor, just above the taskbar
        var displayArea = Microsoft.UI.Windowing.DisplayArea.Primary;
        if (displayArea != null)
        {
            var workArea = displayArea.WorkArea;
            int windowHeight = 340;
            int x = workArea.X;
            int y = workArea.Y + workArea.Height - windowHeight;
            _window.AppWindow.Move(new Windows.Graphics.PointInt32 { X = x, Y = y });
        }
        else
        {
            _window.AppWindow.Move(new Windows.Graphics.PointInt32 { X = 200, Y = 200 });
        }

        // Ensure resources are available
        if (_window.Content is FrameworkElement root)
        {
            root.Resources["BoolToErrorBrushConverter"] = new BoolToErrorBrushConverter();
            root.Resources["EmptyStringToVisibilityConverter"] = new EmptyStringToVisibilityConverter();
        }

        if (_window.Content is Frame frame)
        {
            frame.Navigate(typeof(RunPage));
        }

        _window.Activate();
    }
}
