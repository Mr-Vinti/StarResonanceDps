using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Extensions.Logging;
using StarResonanceDpsAnalysis.WPF.Config;
using StarResonanceDpsAnalysis.WPF.ViewModels;

namespace StarResonanceDpsAnalysis.WPF.Services;

public sealed class GlobalHotkeyService(
    ILogger<GlobalHotkeyService> logger,
    IWindowManagementService windowManager,
    IConfigManager configManager,
    IMousePenetrationService mousePenetration,
    ITopmostService topmostService,
    DpsStatisticsViewModel dpsStatisticsViewModel)
    : IGlobalHotkeyService
{
    private const int WM_HOTKEY = 0x0312;
    private const int HOTKEY_ID_MOUSETHROUGH = 0x1001;
    private const int HOTKEY_ID_TOPMOST = 0x1002;
    private const int HOTKEY_ID_RESET_STATISTIC = 0x1003;
    private AppConfig _config = configManager.CurrentConfig;

    private HwndSource? _source;

    public void Start()
    {
        AttachMessageHook();
        RegisterAll();
        configManager.ConfigurationUpdated += OnConfigUpdated;
    }

    public void Stop()
    {
        try
        {
            UnregisterAll();
        }
        finally
        {
            DetachMessageHook();
            configManager.ConfigurationUpdated -= OnConfigUpdated;
        }
    }

    public void UpdateFromConfig(AppConfig config)
    {
        _config = config;
        // Re-register hotkeys to reflect new key or modifiers
        UnregisterAll();
        RegisterAll();
    }

    private void OnConfigUpdated(object? sender, AppConfig e)
    {
        UpdateFromConfig(e);
    }

    private void AttachMessageHook()
    {
        if (_source is not null) return;
        var window = windowManager.DpsStatisticsView; // host window for message pump
        var helper = new WindowInteropHelper(window);
        var handle = helper.EnsureHandle();
        _source = HwndSource.FromHwnd(handle);
        if (_source == null)
        {
            logger.LogWarning(
                "Failed to obtain HwndSource from handle {Handle} for window {WindowType}. Global hotkeys will be unavailable.",
                handle,
                window.GetType().Name);
        }
        else
        {
            _source.AddHook(WndProc);
        }
    }

    private void DetachMessageHook()
    {
        if (_source is null) return;
        _source.RemoveHook(WndProc);
        _source = null;
    }

    private void RegisterAll()
    {
        try
        {
            RegisterMouseThroughHotkey();
            RegisterTopmostHotkey();
            RegisterResetDpsStatistic();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "RegisterAll hotkeys failed");
        }
    }

    private void UnregisterAll()
    {
        try
        {
            var hWnd = _source?.Handle ?? IntPtr.Zero;
            if (hWnd != IntPtr.Zero)
            {
                UnregisterHotKey(hWnd, HOTKEY_ID_MOUSETHROUGH);
                UnregisterHotKey(hWnd, HOTKEY_ID_TOPMOST);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "UnregisterAll hotkeys failed");
        }
    }

    private void RegisterMouseThroughHotkey()
    {
        var key = _config.MouseThroughShortcut.Key;
        var mods = _config.MouseThroughShortcut.Modifiers;
        if (key == Key.None) return;

        var (vk, fsMods) = ToNative(key, mods);
        var hWnd = _source?.Handle ?? IntPtr.Zero;
        if (hWnd == IntPtr.Zero) return;

        UnregisterHotKey(hWnd, HOTKEY_ID_MOUSETHROUGH);
        if (!RegisterHotKey(hWnd, HOTKEY_ID_MOUSETHROUGH, fsMods, vk))
        {
            logger.LogWarning("RegisterHotKey failed for MouseThrough: {Key}+{Mods}", key, mods);
        }
    }

    private void RegisterTopmostHotkey()
    {
        var key = _config.TopmostShortcut.Key;
        var mods = _config.TopmostShortcut.Modifiers;
        if (key == Key.None) return;

        var (vk, fsMods) = ToNative(key, mods);
        var hWnd = _source?.Handle ?? IntPtr.Zero;
        if (hWnd == IntPtr.Zero) return;

        UnregisterHotKey(hWnd, HOTKEY_ID_TOPMOST);
        if (!RegisterHotKey(hWnd, HOTKEY_ID_TOPMOST, fsMods, vk))
        {
            logger.LogWarning("RegisterHotKey failed for Topmost: {Key}+{Mods}", key, mods);
        }
    }

    private void RegisterResetDpsStatistic()
    {
        var key = _config.ClearDataShortcut.Key;
        var mods = _config.ClearDataShortcut.Modifiers;
        if (key == Key.None) return;

        var (vk, fsMods) = ToNative(key, mods);
        var hWnd = _source?.Handle ?? IntPtr.Zero;
        if (hWnd == IntPtr.Zero) return;

        UnregisterHotKey(hWnd, HOTKEY_ID_RESET_STATISTIC);
        if (!RegisterHotKey(hWnd, HOTKEY_ID_RESET_STATISTIC, fsMods, vk))
        {
            logger.LogWarning("RegisterHotKey failed for Topmost: {Key}+{Mods}", key, mods);
        }
    }

    private static (uint vk, uint fsMods) ToNative(Key key, ModifierKeys mods)
    {
        var vk = (uint)KeyInterop.VirtualKeyFromKey(key);
        uint fs = 0;
        if (mods.HasFlag(ModifierKeys.Alt)) fs |= 0x0001; // MOD_ALT
        if (mods.HasFlag(ModifierKeys.Control)) fs |= 0x0002; // MOD_CONTROL
        if (mods.HasFlag(ModifierKeys.Shift)) fs |= 0x0004; // MOD_SHIFT
        // ignore windows key by design
        return (vk, fs);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WM_HOTKEY) return IntPtr.Zero;

        var id = wParam.ToInt32();
        switch (id)
        {
            case HOTKEY_ID_MOUSETHROUGH:
                ToggleMouseThrough();
                handled = true;
                break;
            case HOTKEY_ID_TOPMOST:
                ToggleTopmost();
                handled = true;
                break;
            case HOTKEY_ID_RESET_STATISTIC:
                TriggerReset();
                handled = true;
                break;
        }

        return IntPtr.Zero;
    }

    private void ToggleMouseThrough()
    {
        try
        {
            var window = windowManager.DpsStatisticsView;
            var newState = !_config.MouseThroughEnabled;
            _config.MouseThroughEnabled = newState;
            mousePenetration.SetMousePenetrate(window, newState);
            _ = configManager.SaveAsync(_config); // persist asynchronously
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ToggleMouseThrough failed");
        }
    }

    private void ToggleTopmost()
    {
        try
        {
            var window = windowManager.DpsStatisticsView;
            var newState = !window.Topmost; // source of truth is window state
            topmostService.SetTopmost(window, newState);
            _config.TopmostEnabled = newState;
            _ = configManager.SaveAsync(_config);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ToggleTopmost failed");
        }
    }

    private void TriggerReset()
    {
        try
        {
            dpsStatisticsViewModel.ResetSection();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "TriggerReset failed");
        }
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}