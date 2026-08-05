using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Notifications;

namespace WatchLog.Api.Controllers;

[Authorize]
public class NotificationsController(INotificationService notificationService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> Get([FromQuery] bool unreadOnly = false, CancellationToken ct = default) =>
        Ok(await notificationService.GetForUserAsync(CurrentUserId, unreadOnly, ct));

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        await notificationService.MarkReadAsync(CurrentUserId, id, ct);
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        await notificationService.MarkAllReadAsync(CurrentUserId, ct);
        return NoContent();
    }
}
