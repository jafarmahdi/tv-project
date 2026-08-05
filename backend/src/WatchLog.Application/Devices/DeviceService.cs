using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;

namespace WatchLog.Application.Devices;

public class DeviceService(IUnitOfWork unitOfWork) : IDeviceService
{
    public async Task<DeviceDto> RegisterAsync(Guid userId, RegisterDeviceRequest request, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<Device>();
        var existing = await repo.Query()
            .FirstOrDefaultAsync(d => d.UserId == userId && d.Platform == request.Platform && d.PushToken == request.PushToken, ct);

        var isNew = existing is null;
        existing ??= new Device { UserId = userId, Platform = request.Platform };
        existing.PushToken = request.PushToken;
        existing.DeviceName = request.DeviceName;
        existing.LastSeenAt = DateTimeOffset.UtcNow;

        if (isNew) await repo.AddAsync(existing, ct);
        else repo.Update(existing);
        await unitOfWork.SaveChangesAsync(ct);

        return new DeviceDto(existing.Id, existing.Platform, existing.DeviceName, existing.LastSeenAt);
    }

    public async Task<IReadOnlyList<DeviceDto>> GetForUserAsync(Guid userId, CancellationToken ct = default) =>
        await unitOfWork.Repository<Device>().Query()
            .Where(d => d.UserId == userId)
            .Select(d => new DeviceDto(d.Id, d.Platform, d.DeviceName, d.LastSeenAt))
            .ToListAsync(ct);
}
