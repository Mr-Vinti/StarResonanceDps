using SharpPcap;

namespace StarResonanceDpsAnalysis.WPF.Services;

public class DeviceManagementService(CaptureDeviceList captureDeviceList) : IDeviceManagementService
{
    public async Task<List<(string name, string description)>> GetNetworkAdaptersAsync()
    {
        return await Task.FromResult(captureDeviceList.Select(device => (device.Name, device.Description)).ToList());
    }
}