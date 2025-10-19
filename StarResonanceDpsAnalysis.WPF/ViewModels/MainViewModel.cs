using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarResonanceDpsAnalysis.WPF.Services;
using StarResonanceDpsAnalysis.WPF.Themes;

namespace StarResonanceDpsAnalysis.WPF.ViewModels;

public partial class MainViewModel(
    ApplicationThemeManager themeManager,
    DebugFunctions debugFunctions,
    IWindowManagementService windowManagement,
    IApplicationControlService appControlService) : BaseViewModel
{
    [ObservableProperty] private List<ApplicationTheme> _availableThemes =
        [ApplicationTheme.Light, ApplicationTheme.Dark];

    [ObservableProperty] private ApplicationTheme _theme = themeManager.GetAppTheme();
    public DebugFunctions Debug { get; init; } = debugFunctions;

    partial void OnThemeChanged(ApplicationTheme value)
    {
        themeManager.Apply(value);
    }

    [RelayCommand]
    private void CallDpsStatisticsView()
    {
        windowManagement.DpsStatisticsView.Show();
    }

    [RelayCommand]
    private void CallSettingsView()
    {
        windowManagement.SettingsView.Show();
    }

    [RelayCommand]
    private void CallSkillBreakdownView()
    {
        windowManagement.SkillBreakdownView.Show();
    }

    [RelayCommand]
    private void CallAboutView()
    {
        windowManagement.AboutView.ShowDialog();
    }

    [RelayCommand]
    private void CallDamageReferenceView()
    {
        windowManagement.DamageReferenceView.Show();
    }

    [RelayCommand]
    private void CallModuleSolveView()
    {
        windowManagement.ModuleSolveView.Show();
    }

    [RelayCommand]
    private void CallBossTrackerView() 
    {
        windowManagement.BossTrackerView.Show();
    }

    [RelayCommand]
    private void Shutdown()
    {
        appControlService.Shutdown();
    }
}