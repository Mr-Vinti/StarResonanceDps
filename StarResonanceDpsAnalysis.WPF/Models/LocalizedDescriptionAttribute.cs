using System.ComponentModel;
using StarResonanceDpsAnalysis.WPF.Localization;

namespace StarResonanceDpsAnalysis.WPF.Models;

/// <summary>
/// Custom attribute for localized display names
/// </summary>
/// <param name="resourceKey"></param>
public class LocalizedDescriptionAttribute(string resourceKey) : DescriptionAttribute
{
    public override string Description => LocalizationManager.GetString(resourceKey);
}