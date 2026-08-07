import '../models/engagement_models.dart';
import 'api_client.dart';

class RatingsApi {
  final ApiClient _client;
  RatingsApi(this._client);

  Future<RatingSummary> getSummary({
    required TargetType targetType,
    required String targetId,
  }) => _client.guard(() async {
    final response = await _client.dio.get(
      '/api/v1/ratings/${targetType.toJson()}/$targetId',
    );
    return RatingSummary.fromJson(response.data as Map<String, dynamic>);
  });

  Future<void> rate({
    required TargetType targetType,
    required String targetId,
    required int score,
  }) => _client.guard(
    () => _client.dio.post(
      '/api/v1/ratings',
      data: {
        'targetType': targetType.toJson(),
        'targetId': targetId,
        'score': score,
      },
    ),
  );
}
