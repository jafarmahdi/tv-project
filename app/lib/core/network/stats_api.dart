import 'api_client.dart';
import '../models/stats_models.dart';

class StatsApi {
  final ApiClient _client;
  StatsApi(this._client);

  Future<UserStats> getMyStats() => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/stats/me');
        return UserStats.fromJson(response.data as Map<String, dynamic>);
      });
}
