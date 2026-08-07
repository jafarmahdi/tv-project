import '../models/engagement_models.dart';
import 'api_client.dart';

class SocialApi {
  final ApiClient _client;
  SocialApi(this._client);

  Map<String, dynamic>? _parentCommentField(String? parentCommentId) =>
      parentCommentId == null ? null : {'parentCommentId': parentCommentId};

  Future<List<CommentEntry>> getComments({
    required TargetType targetType,
    required String targetId,
  }) => _client.guard(() async {
    final response = await _client.dio.get(
      '/api/v1/social/comments/${targetType.toJson()}/$targetId',
    );
    return (response.data as List<dynamic>)
        .map((e) => CommentEntry.fromJson(e as Map<String, dynamic>))
        .toList();
  });

  Future<CommentEntry> addComment({
    required TargetType targetType,
    required String targetId,
    required String body,
    String? parentCommentId,
  }) => _client.guard(() async {
    final response = await _client.dio.post(
      '/api/v1/social/comments',
      data: {
        'targetType': targetType.toJson(),
        'targetId': targetId,
        'body': body,
        ...?_parentCommentField(parentCommentId),
      },
    );
    return CommentEntry.fromJson(response.data as Map<String, dynamic>);
  });
}
