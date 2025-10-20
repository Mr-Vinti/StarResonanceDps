using StarResonanceDpsAnalysis.WPF.Plugins.Interfaces;
using StarResonanceDpsAnalysis.WPF.Services;

namespace StarResonanceDpsAnalysis.WPF.Plugins.BuiltIn;

internal class ModuleSolverPlugin : IPlugin
{
    private readonly IWindowManagementService _windowManagementService;

    public ModuleSolverPlugin(IWindowManagementService windowManagementService)
    {
        _windowManagementService = windowManagementService;
    }

    public string PackageName => "StarResonanceDpsAnalysis.WPF.Plugins.BuiltIn.ModuleSolverPlugin";

    public string PackageVersion => "3.0.0";

    public string GetPluginName(string calture) =>
        PluginLocalizationHelper.GetString("MainView_Plugin_ModuleSolver_Title", calture);

    public string GetPluginDescription(string calture) =>
        PluginLocalizationHelper.GetString("MainView_Plugin_ModuleSolver_Description", calture);

    public void OnRequestRun()
    {
        _windowManagementService.ModuleSolveView.Show();
    }

    public void OnRequestSetting()
    {
        _windowManagementService.SettingsView.Show();
    }
}
