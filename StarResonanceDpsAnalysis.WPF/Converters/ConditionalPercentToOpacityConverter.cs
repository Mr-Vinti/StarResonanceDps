using System.Globalization;
using System.Windows.Data;

namespace StarResonanceDpsAnalysis.WPF.Converters;

/// <summary>
/// Converts (opacityPercent, isEnabled) -> opacity double (0..1).
/// Returns 1.0 when isEnabled is false.
/// </summary>
public sealed class ConditionalPercentToOpacityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Length < 2)
            return 1d;

        var opacityPercent = values[0];
        var enabled = values[1] as bool? ?? (values[1] is string s && bool.TryParse(s, out var b) ? b : false);

        if (!enabled)
            return 1d;

        // Scale percent to 0..1, support double/int/string like PercentToOpacityConverter
        if (opacityPercent is double d) return Math.Clamp(d / 100d, 0d, 1d);
        if (opacityPercent is int i) return Math.Clamp(i / 100d, 0d, 1d);
        if (opacityPercent is string str && double.TryParse(str, NumberStyles.Any, culture, out var parsed))
            return Math.Clamp(parsed / 100d, 0d, 1d);

        return 1d;
    }

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
