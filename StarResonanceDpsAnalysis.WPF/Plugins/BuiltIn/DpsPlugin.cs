using StarResonanceDpsAnalysis.WPF.Plugins.Interfaces;
using StarResonanceDpsAnalysis.WPF.Services;

namespace StarResonanceDpsAnalysis.WPF.Plugins.BuiltIn;

internal class DpsPlugin : IPlugin
{
    private readonly IWindowManagementService _windowManagementService;

    public DpsPlugin(IWindowManagementService windowManagementService)
    {
        _windowManagementService = windowManagementService;
    }

    public string PackageName => "StarResonanceDpsAnalysis.WPF.Plugins.BuiltIn.DpsPlugin";

    public string PackageVersion => "3.0.0";

    public string GetPluginName(string calture) =>
        PluginLocalizationHelper.GetString("MainView_Plugin_DpsTool_Title", calture);

    public string GetPluginDescription(string calture) =>
        PluginLocalizationHelper.GetString("MainView_Plugin_DpsTool_Description", calture);

    public void OnRequestRun()
    {
        _windowManagementService.DpsStatisticsView.Show();
    }

    public void OnRequestSetting()
    {
        _windowManagementService.SettingsView.Show();
    }
}
