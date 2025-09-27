namespace StarResonanceDpsAnalysis.WPF.Services;

public interface IDeviceManagementService
{
    Task<List<(string name, string description)>> GetNetworkAdaptersAsync();
}