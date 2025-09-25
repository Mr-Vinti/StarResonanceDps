using Microsoft.Extensions.DependencyInjection;
using StarResonanceDpsAnalysis.WPF.Views;

namespace StarResonanceDpsAnalysis.WPF.Services;

public class WindowManagementService(IServiceProvider provider) : IWindowManagementService
{
    private DpsStatisticsView? _dpsStatisticsView;
    private SettingsView? _settingsView;
    private SkillBreakdownView? _skillBreakDownView;
    private AboutView? _aboutView;
    private DamageReferenceView? _damageReferenceView;
    private ModuleSolveView? _moduleSolveView;

    public DpsStatisticsView DpsStatisticsView => _dpsStatisticsView ??= CreateDpsStatisticsView();
    public SettingsView SettingsView => _settingsView ??= CreateSettingsView();
    public SkillBreakdownView SkillBreakdownView => _skillBreakDownView ??= CreateSkillBreakDownView();
    public AboutView AboutView => _aboutView ??= CreateAboutView();
    public DamageReferenceView DamageReferenceView => _damageReferenceView ??= CreateDamageReferenceView();
    public ModuleSolveView ModuleSolveView => _moduleSolveView ??= CreateModuleSolveView();

    private DpsStatisticsView CreateDpsStatisticsView()
    {
        var view = provider.GetRequiredService<DpsStatisticsView>();
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
        view.Closed += (_, _) =>
        {
            if (_settingsView == view) _settingsView = null;
        };
        return view;
    }

    private SkillBreakdownView CreateSkillBreakDownView()
    {
        var view = provider.GetRequiredService<SkillBreakdownView>();
        view.Closed += (_, _) =>
        {
            if (_skillBreakDownView == view) _skillBreakDownView = null;
        };
        return view;
    }

    private AboutView CreateAboutView() 
    {
        var view = provider.GetRequiredService<AboutView>();
        view.Closed += (_, _) =>
        {
            if (_aboutView == view) _aboutView = null;
        };
        return view;
    }

    private DamageReferenceView CreateDamageReferenceView()
    {
        var view = provider.GetRequiredService<DamageReferenceView>();
        view.Closed += (_, _) =>
        {
            if (_damageReferenceView == view) _damageReferenceView = null;
        };
        return view;
    }

    private ModuleSolveView CreateModuleSolveView()
    {
        var view = provider.GetRequiredService<ModuleSolveView>();
        view.Closed += (_, _) =>
        {
            if (_moduleSolveView == view) _moduleSolveView = null;
        };
        return view;
    }
}

public interface IWindowManagementService
{
    DpsStatisticsView DpsStatisticsView { get; }
    SettingsView SettingsView { get; }
    SkillBreakdownView SkillBreakdownView { get; }
    AboutView AboutView { get; }
    DamageReferenceView DamageReferenceView { get; }
    ModuleSolveView ModuleSolveView { get; }
}

public static class WindowManagementServiceExtensions
{
    public static IServiceCollection AddWindowManagementService(this IServiceCollection services)
    {
        services.AddSingleton<IWindowManagementService, WindowManagementService>();
        return services;
    }
}