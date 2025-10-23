using System.Globalization;
using StarResonanceDpsAnalysis.WPF.Localization;
using StarResonanceDpsAnalysis.WPF.Plugins.Interfaces;
using StarResonanceDpsAnalysis.WPF.Properties;
using StarResonanceDpsAnalysis.WPF.Services;

namespace StarResonanceDpsAnalysis.WPF.Plugins.BuiltIn;

internal class WorldBossPlugin : IPlugin
{
    private readonly IWindowManagementService _windowManagementService;
    private readonly LocalizationManager _localizationManager;

    public WorldBossPlugin(IWindowManagementService windowManagementService, LocalizationManager localizationManager)
    {
        _windowManagementService = windowManagementService;
        _localizationManager = localizationManager;
    }

    public string PackageName => "StarResonanceDpsAnalysis.WPF.Plugins.BuiltIn.WorldBossPlugin";

    public string PackageVersion => "3.0.0";

    public string GetPluginName(CultureInfo cultureInfo) =>
        _localizationManager.GetString(ResourcesKeys.MainView_Plugin_WorldBoss_Title, cultureInfo);

    public string GetPluginDescription(CultureInfo cultureInfo) =>
        _localizationManager.GetString(ResourcesKeys.MainView_Plugin_WorldBoss_Description, cultureInfo);

    public void OnRequestRun()
    {
        _windowManagementService.BossTrackerView.Show();
    }

    public void OnRequestSetting()
    {
        _windowManagementService.SettingsView.Show();
    }
}
