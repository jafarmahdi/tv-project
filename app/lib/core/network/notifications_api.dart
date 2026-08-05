import 'api_client.dart';
import '../models/notification_models.dart';

class NotificationsApi {
  final ApiClient _client;
  NotificationsApi(this._client);

  Future<List<AppNotification>> getAll({bool unreadOnly = false}) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/notifications', queryParameters: {'unreadOnly': unreadOnly});
        return (response.data as List<dynamic>).map((e) => AppNotification.fromJson(e as Map<String, dynamic>)).toList();
      });

  Future<void> markRead(String id) => _client.guard(() => _client.dio.post('/api/v1/notifications/$id/read'));

  Future<void> markAllRead() => _client.guard(() => _client.dio.post('/api/v1/notifications/read-all'));
}
