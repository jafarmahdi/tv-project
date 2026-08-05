using WatchLog.Domain.Enums;

namespace WatchLog.Application.Devices;

public record RegisterDeviceRequest(DevicePlatform Platform, string? PushToken, string? DeviceName);
public record DeviceDto(Guid Id, DevicePlatform Platform, string? DeviceName, DateTimeOffset LastSeenAt);
