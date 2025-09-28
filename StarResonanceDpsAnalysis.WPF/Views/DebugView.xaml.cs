using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using StarResonanceDpsAnalysis.WPF.ViewModels;

namespace StarResonanceDpsAnalysis.WPF.Views;

/// <summary>
/// Interaction logic for DebugView.xaml
/// </summary>
public partial class DebugView : Window
{
    private bool _isAutoScrollPending;

    public DebugView(DebugFunctions debugFunctions)
    {
        InitializeComponent();
        DataContext = debugFunctions;

        // Auto scroll when new logs are added
        debugFunctions.LogAdded += OnLogAdded;
    }

    private void OnLogAdded(object? sender, EventArgs e)
    {
        if (DataContext is DebugFunctions debugFunctions && debugFunctions.AutoScrollEnabled && !_isAutoScrollPending)
        {
            _isAutoScrollPending = true;
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    if (FindName("LogListBox") is ListBox listBox && listBox.Items.Count > 0)
                    {
                        listBox.ScrollIntoView(listBox.Items[^1]);
                    }
                }
                finally
                {
                    _isAutoScrollPending = false;
                }
            }, DispatcherPriority.Background);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is DebugFunctions debugFunctions)
        {
            debugFunctions.LogAdded -= OnLogAdded;
        }

        base.OnClosed(e);
    }
}

/// <summary>
/// Converter for log level to brush color with caching for better performance
/// </summary>
public class LogLevelToBrushConverter : IValueConverter
{
    public static readonly LogLevelToBrushConverter Instance = new();

    // Cache brushes to avoid creating new instances every time
    private static readonly Dictionary<LogLevel, SolidColorBrush> BrushCache = new()
    {
        [LogLevel.Trace] = new SolidColorBrush(Colors.Gray),
        [LogLevel.Debug] = new SolidColorBrush(Colors.DarkBlue),
        [LogLevel.Information] = new SolidColorBrush(Colors.Green),
        [LogLevel.Warning] = new SolidColorBrush(Colors.Orange),
        [LogLevel.Error] = new SolidColorBrush(Colors.Red),
        [LogLevel.Critical] = new SolidColorBrush(Colors.DarkRed)
    };

    private static readonly SolidColorBrush DefaultBrush = new(Colors.Black);

    static LogLevelToBrushConverter()
    {
        // Freeze brushes for better performance
        foreach (var brush in BrushCache.Values)
        {
            brush.Freeze();
        }

        DefaultBrush.Freeze();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LogLevel level && BrushCache.TryGetValue(level, out var brush))
        {
            return brush;
        }

        return DefaultBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter for log level to background color with caching for better performance
/// </summary>
public class LogLevelToBackgroundConverter : IValueConverter
{
    public static readonly LogLevelToBackgroundConverter Instance = new();

    // Cache background brushes
    private static readonly Dictionary<LogLevel, SolidColorBrush> BackgroundCache = new()
    {
        [LogLevel.Error] = new SolidColorBrush(Color.FromRgb(255, 245, 245)),
        [LogLevel.Critical] = new SolidColorBrush(Color.FromRgb(255, 235, 235)),
        [LogLevel.Warning] = new SolidColorBrush(Color.FromRgb(255, 250, 240))
    };

    private static readonly SolidColorBrush TransparentBrush = Brushes.Transparent;

    static LogLevelToBackgroundConverter()
    {
        // Freeze brushes for better performance
        foreach (var brush in BackgroundCache.Values)
        {
            brush.Freeze();
        }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LogLevel level && BackgroundCache.TryGetValue(level, out var brush))
        {
            return brush;
        }

        return TransparentBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter for exception to string with null handling
/// </summary>
public class ExceptionToStringConverter : IValueConverter
{
    public static readonly ExceptionToStringConverter Instance = new();

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Exception ex)
        {
            return $"Exception: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}