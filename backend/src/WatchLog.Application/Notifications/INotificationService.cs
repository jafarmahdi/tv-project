using WatchLog.Domain.Enums;

namespace WatchLog.Application.Notifications;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetForUserAsync(Guid userId, bool unreadOnly, CancellationToken ct = default);
    Task MarkReadAsync(Guid userId, Guid notificationId, CancellationToken ct = default);
    Task MarkAllReadAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Persists the notification and pushes it live over SignalR to the user's connected clients.</summary>
    Task<NotificationDto> CreateAndPushAsync(Guid userId, NotificationType type, string title, string body, string? dataJson = null, CancellationToken ct = default);
}
