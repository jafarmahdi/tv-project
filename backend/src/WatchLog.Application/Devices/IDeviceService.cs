namespace WatchLog.Application.Devices;

public interface IDeviceService
{
    Task<DeviceDto> RegisterAsync(Guid userId, RegisterDeviceRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<DeviceDto>> GetForUserAsync(Guid userId, CancellationToken ct = default);
}
