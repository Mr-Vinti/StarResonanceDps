using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using StarResonanceDpsAnalysis.WPF.Models;
using StarResonanceDpsAnalysis.WPF.Properties;
using WPFLocalizeExtension.Engine;

namespace StarResonanceDpsAnalysis.WPF.Localization;

public static class LocalizationManager
{
    private static bool _initialized;
    private static readonly CultureInfo _systemDefaultCultureInfo;
    private static readonly string DefaultAssemblyName;
    private const string DefaultDictionaryName = "Properties.Resources";

    static LocalizationManager()
    {
        _systemDefaultCultureInfo = CultureInfo.CurrentUICulture;
        var assemblyName = typeof(App).Assembly.GetName().Name;
        DefaultAssemblyName = string.IsNullOrWhiteSpace(assemblyName)
            ? Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty
            : assemblyName;
        ConfigureLocalizationExtension();
    }

    public static event EventHandler<CultureInfo>? CultureChanged;

    public static void Initialize(Language language)
    {
        if (_initialized) return;

        ApplyLanguage(language);
        _initialized = true;
    }

    public static void ApplyLanguage(Language language)
    {
        var targetCulture = ResolveCulture(language);
        ApplyCulture(targetCulture);
    }

    public static CultureInfo ResolveCulture(Language language)
    {
        if (language == Language.Auto)
        {
            return _systemDefaultCultureInfo;
        }

        try
        {
            var ret = language.GetCultureInfo();
            Debug.Assert(ret != null, nameof(ret) + " != null");
            return ret;
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.CurrentUICulture;
        }
    }

    private static void ConfigureLocalizationExtension()
    {
        // LocalizeDictionary.Instance.DefaultAssembly = DefaultAssemblyName;
        // LocalizeDictionary.Instance.DefaultDictionary = DefaultDictionaryName;
        LocalizeDictionary.Instance.IncludeInvariantCulture = false;
        LocalizeDictionary.Instance.SetCurrentThreadCulture = false;
    }

    private static void ApplyCulture(CultureInfo culture)
    {
        LocalizeDictionary.Instance.Culture = culture;
        Resources.Culture = culture;
        SetThreadCulture(culture);
        OnCultureChanged(culture);
    }

    private static void SetThreadCulture(CultureInfo culture)
    {
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    private static void OnCultureChanged(CultureInfo e)
    {
        CultureChanged?.Invoke(null, e);
    }

    public static string GetString(string key)
    {
        var culture = LocalizeDictionary.Instance.Culture ?? CultureInfo.CurrentUICulture;
        var localized = Resources.ResourceManager.GetString(key, culture);
        return !string.IsNullOrEmpty(localized)
            ? localized!
            : key;
    }
}
