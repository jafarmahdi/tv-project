using WatchLog.Domain.Enums;

namespace WatchLog.Application.Notifications;

public record NotificationDto(Guid Id, NotificationType Type, string Title, string Body, bool IsRead, DateTimeOffset CreatedAt);
