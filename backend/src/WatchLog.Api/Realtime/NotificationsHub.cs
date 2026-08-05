using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Realtime;

/// <summary>
/// Real-time channel for notifications and friends' activity. Clients connect authenticated
/// (JWT via `?access_token=`, see `Program.cs`) and are auto-joined to a per-user group so the
/// server can target pushes without the client managing subscriptions.
/// </summary>
[Authorize]
public class NotificationsHub(ICurrentUserService currentUser) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (currentUser.UserId is { } userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(userId));
        }
        await base.OnConnectedAsync();
    }

    public static string GroupName(Guid userId) => $"user:{userId}";
}
