using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarResonanceDpsAnalysis.Core.Extends.System;
using StarResonanceDpsAnalysis.WPF.Config;
using StarResonanceDpsAnalysis.WPF.Localization;
using StarResonanceDpsAnalysis.WPF.Models;
using StarResonanceDpsAnalysis.WPF.Services;
using AppConfig = StarResonanceDpsAnalysis.WPF.Config.AppConfig;
using KeyBinding = StarResonanceDpsAnalysis.WPF.Models.KeyBinding;

namespace StarResonanceDpsAnalysis.WPF.ViewModels;

public partial class SettingsViewModel(
    IConfigManager configManger,
    IDeviceManagementService deviceManagementService)
    : BaseViewModel
{
    [ObservableProperty] private AppConfig _appConfig = null!;
    [ObservableProperty]
    private List<Option<Language>> _availableLanguages = [
            new(Language.Auto, Language.Auto.GetLocalizedDescription()),
            new(Language.ZhCn, Language.ZhCn.GetLocalizedDescription()),
            new(Language.EnUs, Language.EnUs.GetLocalizedDescription())
    ];
    [ObservableProperty]
    private List<NetworkAdapterInfo> _availableNetworkAdapters = [];
    [ObservableProperty]
    private List<Option<NumberDisplayMode>> _availableNumberDisplayModes = [
            new(NumberDisplayMode.Wan, NumberDisplayMode.Wan.GetLocalizedDescription()),
            new(NumberDisplayMode.KMB, NumberDisplayMode.KMB.GetLocalizedDescription())
    ];

    private bool _cultureHandlerSubscribed;

    // [ObservableProperty] private Option<Language> _selectedLanguage;
    [ObservableProperty] private Option<Language>? _selectedLanguage;
    [ObservableProperty] private Option<NumberDisplayMode>? _selectedNumberDisplayMode;

    public event Action? RequestClose;

    partial void OnAppConfigChanging(AppConfig value)
    {
        if (value is null)
        {
            return;
        }

        value.PropertyChanged -= OnAppConfigPropertyChanged;
    }

    partial void OnAppConfigChanged(AppConfig value)
    {
        if (value is null)
        {
            return;
        }

        value.PropertyChanged += OnAppConfigPropertyChanged;

        LocalizationManager.ApplyLanguage(value.Language);
        UpdateLanguageDependentCollections();
        SyncOptions();
    }

    partial void OnSelectedNumberDisplayModeChanged(Option<NumberDisplayMode>? value)
    {
        if (value == null) return;
        AppConfig.DamageDisplayType = value.Value;
    }

    partial void OnSelectedLanguageChanged(Option<Language>? value)
    {
        if (value == null) return;
        AppConfig.Language = value.Value;
        LocalizationManager.ApplyLanguage(value.Value);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task LoadedAsync()
    {
        AppConfig = configManger.CurrentConfig.Clone();
        if (!_cultureHandlerSubscribed)
        {
            LocalizationManager.CultureChanged += OnCultureChanged;
            _cultureHandlerSubscribed = true;
        }

        UpdateLanguageDependentCollections();
        // SyncSelectedLanguage();
        LocalizationManager.ApplyLanguage(AppConfig.Language);
        await LoadNetworkAdaptersAsync();
    }

    private async Task LoadNetworkAdaptersAsync()
    {
        var adapters = await deviceManagementService.GetNetworkAdaptersAsync();
        AvailableNetworkAdapters = adapters.Select(a => new NetworkAdapterInfo(a.name, a.description)).ToList();
        AppConfig.PreferredNetworkAdapter =
            AvailableNetworkAdapters.FirstOrDefault(a => a.Name == AppConfig.PreferredNetworkAdapter?.Name);
    }

    /// <summary>
    /// Handle shortcut key input for mouse through shortcut
    /// </summary>
    [RelayCommand]
    private void HandleMouseThroughShortcut(object parameter)
    {
        if (parameter is KeyEventArgs e)
        {
            HandleShortcutInput(e, ShortcutType.MouseThrough);
        }
    }

    /// <summary>
    /// Handle shortcut key input for clear data shortcut
    /// </summary>
    /// <param name="parameter">KeyEventArgs from the view</param>
    [RelayCommand]
    private void HandleClearDataShortcut(object parameter)
    {
        if (parameter is KeyEventArgs e)
        {
            HandleShortcutInput(e, ShortcutType.ClearData);
        }
    }

    private void OnAppConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not AppConfig config)
        {
            return;
        }

        if (e.PropertyName == nameof(AppConfig.Language))
        {
            LocalizationManager.ApplyLanguage(config.Language);
            UpdateLanguageDependentCollections();
        }
    }

    /// <summary>
    /// Generic shortcut input handler
    /// </summary>
    private void HandleShortcutInput(KeyEventArgs e, ShortcutType shortcutType)
    {
        e.Handled = true; // we'll handle the key

        var modifiers = Keyboard.Modifiers;
        var key = e.Key == Key.System ? e.SystemKey : e.Key;

        // Allow Delete to clear - same logic as WinForms
        if (key == Key.Delete)
        {
            ClearShortcut(shortcutType);
            return;
        }

        // Ignore modifier-only presses - same logic as WinForms
        if (key.IsControlKey() || key.IsAltKey() || key.IsShiftKey())
        {
            return;
        }

        // Exclude physical modifier keys from being shown as main key
        if (!key.IsControlKey() && !key.IsAltKey() && !key.IsShiftKey())
        {
            UpdateShortcut(shortcutType, key, modifiers);
        }
    }

    /// <summary>
    /// Update a specific shortcut
    /// </summary>
    private void UpdateShortcut(ShortcutType shortcutType, Key key, ModifierKeys modifiers)
    {
        var shortcutData = new KeyBinding(key, modifiers);

        switch (shortcutType)
        {
            case ShortcutType.MouseThrough:
                AppConfig.MouseThroughShortcut = shortcutData;
                break;
            case ShortcutType.ClearData:
                AppConfig.ClearDataShortcut = shortcutData;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(shortcutType), shortcutType, null);
        }
    }

    /// <summary>
    /// Clear a specific shortcut
    /// </summary>
    private void ClearShortcut(ShortcutType shortcutType)
    {
        var shortCut = new KeyBinding(Key.None, ModifierKeys.None);
        switch (shortcutType)
        {
            case ShortcutType.MouseThrough:
                AppConfig.MouseThroughShortcut = shortCut;
                break;
            case ShortcutType.ClearData:
                AppConfig.ClearDataShortcut = shortCut;
                break;
        }
    }

    public Task ApplySettingsAsync()
    {
        return configManger.SaveAsync(AppConfig);
    }

    [RelayCommand]
    private async Task Confirm()
    {
        await ApplySettingsAsync();
        DetachCultureHandler();
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        DetachCultureHandler();
        RequestClose?.Invoke();
    }

    private void OnCultureChanged(object? sender, CultureInfo culture)
    {
        UpdateLanguageDependentCollections();
    }

    private void DetachCultureHandler()
    {
        if (!_cultureHandlerSubscribed) return;
        LocalizationManager.CultureChanged -= OnCultureChanged;
        _cultureHandlerSubscribed = false;
    }
}

public partial class SettingsViewModel
{
    private static void UpdateEnumList<T>(IEnumerable<Option<T>> list) where T : Enum
    {
        foreach (var itm in list)
        {
            itm.Display = itm.Value.GetLocalizedDescription();
        }
    }

    private void UpdateLanguageDependentCollections()
    {
        UpdateEnumList(AvailableNumberDisplayModes);
        UpdateEnumList(AvailableLanguages);
    }

    private void SyncLanguageOption()
    {
        var (ret, opt) = SyncOption(SelectedLanguage, AvailableLanguages, AppConfig.Language);
        if (ret) SelectedLanguage = opt!;
    }

    private void SyncNumberDisplayModeOption()
    {
        var (ret, opt) = SyncOption(SelectedNumberDisplayMode, AvailableNumberDisplayModes,
            AppConfig.DamageDisplayType);
        if (ret) SelectedNumberDisplayMode = opt!;
    }

    private void SyncOptions()
    {
        SyncLanguageOption();
        SyncNumberDisplayModeOption();
    }

    private static (bool result, Option<T>? opt) SyncOption<T>(Option<T>? option, List<Option<T>> availableList,
    T origin)
    {
        if (Equal(option, origin)) return (false, null);

        var match = availableList.FirstOrDefault(l => Equal(l, origin));
        Debug.Assert(match != null);
        return (true, match);

        bool Equal(Option<T>? o1, T o2)
        {
            return o1?.Value?.Equals(o2) ?? false;
        }
    }
}

public partial class Option<T>(T value, string display) : BaseViewModel
{
    [ObservableProperty] private T _value = value;
    [ObservableProperty] private string _display = display;

    public void Deconstruct(out T value, out string display)
    {
        value = Value;
        display = Display;
    }
}

/// <summary>
/// Enum to identify shortcut types
/// </summary>
public enum ShortcutType
{
    MouseThrough,
    ClearData
}

public sealed class SettingsDesignTimeViewModel : SettingsViewModel
{
    public SettingsDesignTimeViewModel() : base(null!, null!)
    {
        AppConfig = new AppConfig();
        AvailableNetworkAdapters =
        [
            new NetworkAdapterInfo("WAN Adapter", "WAN"),
            new NetworkAdapterInfo("WLAN Adapter", "WLAN")
        ];
        AppConfig.MouseThroughShortcut = new KeyBinding(Key.F6, ModifierKeys.Control);
        AppConfig.ClearDataShortcut = new KeyBinding(Key.F9, ModifierKeys.None);
        AvailableLanguages =
        [
            new Option<Language>(Language.Auto, "Follow System")
        ];
        AvailableNumberDisplayModes =
        [
            new Option<NumberDisplayMode>(NumberDisplayMode.Wan, "四位计数法 (万亿兆)"),
            new Option<NumberDisplayMode>(NumberDisplayMode.KMB, "三位计数法 (KMBT)")
        ];
    }
}
