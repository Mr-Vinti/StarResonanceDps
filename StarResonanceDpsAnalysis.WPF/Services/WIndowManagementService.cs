using Microsoft.Extensions.DependencyInjection;
using StarResonanceDpsAnalysis.WPF.Views;
using System.Windows;

namespace StarResonanceDpsAnalysis.WPF.Services;

public class WindowManagementService(IServiceProvider provider) : IWindowManagementService
{
    private AboutView? _aboutView;
    private DamageReferenceView? _damageReferenceView;
    private DpsStatisticsView? _dpsStatisticsView;
    private ModuleSolveView? _moduleSolveView;
    private SettingsView? _settingsView;
    private SkillBreakdownView? _skillBreakDownView;
    private BossTrackerView? _bossTrackerView;

    public DpsStatisticsView DpsStatisticsView => _dpsStatisticsView ??= CreateDpsStatisticsView();
    public SettingsView SettingsView => _settingsView ??= CreateSettingsView();
    public SkillBreakdownView SkillBreakdownView => _skillBreakDownView ??= CreateSkillBreakDownView();
    public AboutView AboutView => _aboutView ??= CreateAboutView();
    public DamageReferenceView DamageReferenceView => _damageReferenceView ??= CreateDamageReferenceView();
    public ModuleSolveView ModuleSolveView => _moduleSolveView ??= CreateModuleSolveView();
    public BossTrackerView BossTrackerView => _bossTrackerView ??= CreateBossTrackerView();
    public MainView MainView => provider.GetRequiredService<MainView>();

    private static void ConfigureOwnedToolWindow(Window view)
    {
        if (Application.Current?.MainWindow is MainView main && view.Owner == null && view != main)
        {
            view.Owner = main;
            view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        // view.ShowInTaskbar = false; // only one taskbar icon (main)
    }

    private DpsStatisticsView CreateDpsStatisticsView()
    {
        var view = provider.GetRequiredService<DpsStatisticsView>();
        ConfigureOwnedToolWindow(view);
        // When the window is closed, clear the cached reference so a new instance will be created next time.
        view.Closed += (_, _) =>
        {
            if (_dpsStatisticsView == view) _dpsStatisticsView = null;
        };
        return view;
    }

    private SettingsView CreateSettingsView()
    {
        var view = provider.GetRequiredService<SettingsView>();
        ConfigureOwnedToolWindow(view);
        view.Closed += (_, _) =>
        {
            if (_settingsView == view) _settingsView = null;
        };
        return view;
    }

    private SkillBreakdownView CreateSkillBreakDownView()
    {
        var view = provider.GetRequiredService<SkillBreakdownView>();
        ConfigureOwnedToolWindow(view);
        view.Closed += (_, _) =>
        {
            if (_skillBreakDownView == view) _skillBreakDownView = null;
        };
        return view;
    }

    private AboutView CreateAboutView()
    {
        var view = provider.GetRequiredService<AboutView>();
        ConfigureOwnedToolWindow(view);
        view.Closed += (_, _) =>
        {
            if (_aboutView == view) _aboutView = null;
        };
        return view;
    }

    private DamageReferenceView CreateDamageReferenceView()
    {
        var view = provider.GetRequiredService<DamageReferenceView>();
        ConfigureOwnedToolWindow(view);
        view.Closed += (_, _) =>
        {
            if (_damageReferenceView == view) _damageReferenceView = null;
        };
        return view;
    }

    private ModuleSolveView CreateModuleSolveView()
    {
        var view = provider.GetRequiredService<ModuleSolveView>();
        ConfigureOwnedToolWindow(view);
        view.Closed += (_, _) =>
        {
            if (_moduleSolveView == view) _moduleSolveView = null;
        };
        return view;
    }

    private BossTrackerView CreateBossTrackerView()
    {
        var view = provider.GetRequiredService<BossTrackerView>();
        ConfigureOwnedToolWindow(view);
        view.Closed += (_, _) =>
        {
            if (_bossTrackerView == view) _bossTrackerView = null;
        };
        return view;
    }
}

public static class WindowManagementServiceExtensions
{
    public static IServiceCollection AddWindowManagementService(this IServiceCollection services)
    {
        services.AddSingleton<IWindowManagementService, WindowManagementService>();
        return services;
    }
}