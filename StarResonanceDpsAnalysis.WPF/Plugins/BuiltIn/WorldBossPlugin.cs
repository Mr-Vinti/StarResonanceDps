using StarResonanceDpsAnalysis.WPF.Plugins.Interfaces;
using StarResonanceDpsAnalysis.WPF.Services;

namespace StarResonanceDpsAnalysis.WPF.Plugins.BuiltIn;

internal class WorldBossPlugin : IPlugin
{
    private readonly IWindowManagementService _windowManagementService;

    public WorldBossPlugin(IWindowManagementService windowManagementService)
    {
        _windowManagementService = windowManagementService;
    }

    public string PackageName => "StarResonanceDpsAnalysis.WPF.Plugins.BuiltIn.WorldBossPlugin";

    public string PackageVersion => "3.0.0";

    public string GetPluginName(string calture) =>
        PluginLocalizationHelper.GetString("MainView_Plugin_WorldBoss_Title", calture);

    public string GetPluginDescription(string calture) =>
        PluginLocalizationHelper.GetString("MainView_Plugin_WorldBoss_Description", calture);

    public void OnRequestRun()
    {
        _windowManagementService.BossTrackerView.Show();
    }

    public void OnRequestSetting()
    {
        _windowManagementService.SettingsView.Show();
    }
}
