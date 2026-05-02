using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace RunDialog.App.Converters;

public sealed class BoolToErrorBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var isError = value is true;
        if (isError)
        {
            if (Application.Current.Resources.TryGetValue("ErrorBrush", out var brush) && brush is SolidColorBrush sb)
                return sb;
        }
        else
        {
            if (Application.Current.Resources.TryGetValue("TextFillColorPrimaryBrush", out var brush) && brush is SolidColorBrush sb)
                return sb;
        }
        return new SolidColorBrush(isError ? Microsoft.UI.Colors.Crimson : Microsoft.UI.Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
