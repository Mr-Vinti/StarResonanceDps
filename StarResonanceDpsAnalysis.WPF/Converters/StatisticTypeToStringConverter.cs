using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using StarResonanceDpsAnalysis.WPF.Localization;
using StarResonanceDpsAnalysis.WPF.Models;
using StarResonanceDpsAnalysis.WPF.Properties;

namespace StarResonanceDpsAnalysis.WPF.Converters;

internal class StatisticTypeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is StatisticType type)
        {
            return type switch
            {
                StatisticType.Damage => LocalizationManager.GetString(LocalizationKeys.StatisticType_Damage),
                StatisticType.Healing => LocalizationManager.GetString(LocalizationKeys.StatisticType_Healing),
                StatisticType.TakenDamage => LocalizationManager.GetString(LocalizationKeys.StatisticType_TakenDamage),
                StatisticType.NpcTakenDamage => LocalizationManager.GetString(LocalizationKeys.StatisticType_NpcTakenDamage),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported StatisticType")
            };
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Does support convert string back to StatisticType");
    }
}

internal class ScopeTimeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ScopeTime type)
        {
            return type switch
            {
                ScopeTime.Current => LocalizationManager.GetString(LocalizationKeys.ScopeTime_Current),
                ScopeTime.Total => LocalizationManager.GetString(LocalizationKeys.ScopeTime_Total),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported ScopeTime")
            };
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Does support convert string back to StatisticType");
    }
}