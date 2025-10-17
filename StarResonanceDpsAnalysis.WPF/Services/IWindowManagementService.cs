using StarResonanceDpsAnalysis.WPF.Views;

namespace StarResonanceDpsAnalysis.WPF.Services;

public interface IWindowManagementService
{
    DpsStatisticsView DpsStatisticsView { get; }
    SettingsView SettingsView { get; }
    SkillBreakdownView SkillBreakdownView { get; }
    AboutView AboutView { get; }
    DamageReferenceView DamageReferenceView { get; }
    ModuleSolveView ModuleSolveView { get; }
    BossTrackerView BossTrackerView { get; }
}