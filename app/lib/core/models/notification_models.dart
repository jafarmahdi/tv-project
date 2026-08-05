/// Mirrors `WatchLog.Domain.Enums.NotificationType`.
enum NotificationType { newEpisode, newSeason, upcomingRelease, friendActivity, achievement, system }

extension NotificationTypeJson on NotificationType {
  static NotificationType fromJson(int value) => NotificationType.values[value];
}

/// Mirrors `WatchLog.Application.Notifications.NotificationDto`.
class AppNotification {
  final String id;
  final NotificationType type;
  final String title;
  final String body;
  final bool isRead;
  final DateTime createdAt;

  AppNotification({
    required this.id,
    required this.type,
    required this.title,
    required this.body,
    required this.isRead,
    required this.createdAt,
  });

  factory AppNotification.fromJson(Map<String, dynamic> json) => AppNotification(
        id: json['id'] as String,
        type: NotificationTypeJson.fromJson(json['type'] as int),
        title: json['title'] as String,
        body: json['body'] as String,
        isRead: json['isRead'] as bool,
        createdAt: DateTime.parse(json['createdAt'] as String),
      );
}
