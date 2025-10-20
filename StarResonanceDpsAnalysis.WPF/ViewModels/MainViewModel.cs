using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarResonanceDpsAnalysis.WPF.Localization;
using StarResonanceDpsAnalysis.WPF.Properties;
using StarResonanceDpsAnalysis.WPF.Services;
using StarResonanceDpsAnalysis.WPF.Themes;

namespace StarResonanceDpsAnalysis.WPF.ViewModels;

public partial class MainViewModel(
    ApplicationThemeManager themeManager,
    DebugFunctions debugFunctions,
    IWindowManagementService windowManagement,
    IApplicationControlService appControlService,
    ITrayService trayService,
    LocalizationManager localizationManager,
    IMessageDialogService dialogService) : BaseViewModel
{
    [ObservableProperty]
    private List<ApplicationTheme> _availableThemes =
        [ApplicationTheme.Light, ApplicationTheme.Dark];

    [ObservableProperty] private ApplicationTheme _theme = themeManager.GetAppTheme();
    public DebugFunctions Debug { get; init; } = debugFunctions;

    partial void OnThemeChanged(ApplicationTheme value)
    {
        themeManager.Apply(value);
    }

    [RelayCommand]
    private void InitializeTray()
    {
        trayService.Initialize("Star Resonance DPS");
    }

    [RelayCommand]
    private void MinimizeToTray()
    {
        trayService.MinimizeToTray();
    }

    [RelayCommand]
    private void RestoreFromTray()
    {
        trayService.Restore();
    }

    [RelayCommand]
    private void ExitFromTray()
    {
        trayService.Exit();
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
        var title = localizationManager.GetString(ResourcesKeys.App_Exit_Confirm_Title);
        var content = localizationManager.GetString(ResourcesKeys.App_Exit_Confirm_Content);

        var result = dialogService.Show(title, content);
        if (result == true)
        {
            appControlService.Shutdown();
        }
    }
}