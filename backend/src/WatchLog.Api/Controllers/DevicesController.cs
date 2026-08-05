using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Devices;

namespace WatchLog.Api.Controllers;

[Authorize]
public class DevicesController(IDeviceService deviceService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpPost]
    public async Task<ActionResult<DeviceDto>> Register(RegisterDeviceRequest request, CancellationToken ct) =>
        Ok(await deviceService.RegisterAsync(CurrentUserId, request, ct));

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DeviceDto>>> GetMine(CancellationToken ct) =>
        Ok(await deviceService.GetForUserAsync(CurrentUserId, ct));
}
