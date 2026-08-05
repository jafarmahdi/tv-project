using Microsoft.AspNetCore.SignalR;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Realtime;

public class SignalRNotificationPublisher(IHubContext<NotificationsHub> hub) : INotificationPublisher
{
    public Task PushNotificationAsync(Guid userId, object payload, CancellationToken ct = default) =>
        hub.Clients.Group(NotificationsHub.GroupName(userId)).SendAsync("notification", payload, ct);

    public Task PushActivityAsync(Guid userId, object payload, CancellationToken ct = default) =>
        hub.Clients.Group(NotificationsHub.GroupName(userId)).SendAsync("activity", payload, ct);
}
