using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Notifications;

public class NotificationService(IUnitOfWork unitOfWork, INotificationPublisher publisher) : INotificationService
{
    public async Task<IReadOnlyList<NotificationDto>> GetForUserAsync(Guid userId, bool unreadOnly, CancellationToken ct = default)
    {
        var query = unitOfWork.Repository<Notification>().Query().Where(n => n.UserId == userId);
        if (unreadOnly) query = query.Where(n => !n.IsRead);

        return await query.OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(n.Id, n.Type, n.Title, n.Body, n.IsRead, n.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task MarkReadAsync(Guid userId, Guid notificationId, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<Notification>();
        var notification = await repo.Query().FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, ct)
            ?? throw new NotFoundException(nameof(Notification), notificationId);

        notification.IsRead = true;
        repo.Update(notification);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task MarkAllReadAsync(Guid userId, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<Notification>();
        var unread = await repo.Query().Where(n => n.UserId == userId && !n.IsRead).ToListAsync(ct);
        foreach (var n in unread)
        {
            n.IsRead = true;
            repo.Update(n);
        }
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<NotificationDto> CreateAndPushAsync(Guid userId, NotificationType type, string title, string body,
        string? dataJson = null, CancellationToken ct = default)
    {
        var notification = new Notification { UserId = userId, Type = type, Title = title, Body = body, DataJson = dataJson };
        await unitOfWork.Repository<Notification>().AddAsync(notification, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var dto = new NotificationDto(notification.Id, type, title, body, false, notification.CreatedAt);
        await publisher.PushNotificationAsync(userId, dto, ct);
        return dto;
    }
}
