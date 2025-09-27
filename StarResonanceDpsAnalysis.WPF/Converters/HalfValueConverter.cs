using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StarResonanceDpsAnalysis.WPF.Converters;

public class HalfValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d) return new CornerRadius(d / 2);
        return new CornerRadius(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}