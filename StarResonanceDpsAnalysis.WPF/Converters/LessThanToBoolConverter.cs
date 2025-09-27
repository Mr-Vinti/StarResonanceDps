﻿using System.Globalization;
using System.Windows.Data;

namespace StarResonanceDpsAnalysis.WPF.Converters;

public class LessThanToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d) return DoubleConverter(d, parameter);
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static bool DoubleConverter(double value, object? parameter)
    {
        return double.TryParse(parameter?.ToString(), out var th) && value < th;
    }
}