import 'api_client.dart';
import '../models/user_models.dart';

class UsersApi {
  final ApiClient _client;
  UsersApi(this._client);

  Future<MeProfile> getMe() => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/users/me');
        return MeProfile.fromJson(response.data as Map<String, dynamic>);
      });

  Future<MeProfile> updateProfile({
    String? displayName,
    String? avatarUrl,
    String? bio,
    String? locale,
    int? themePreference,
    bool? isPrivate,
  }) =>
      _client.guard(() async {
        final response = await _client.dio.put('/api/v1/users/me', data: {
          if (displayName != null) 'displayName': displayName,
          if (avatarUrl != null) 'avatarUrl': avatarUrl,
          if (bio != null) 'bio': bio,
          if (locale != null) 'locale': locale,
          if (themePreference != null) 'themePreference': themePreference,
          if (isPrivate != null) 'isPrivate': isPrivate,
        });
        return MeProfile.fromJson(response.data as Map<String, dynamic>);
      });

  Future<PublicProfile> getPublicProfile(String userId) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/users/$userId');
        return PublicProfile.fromJson(response.data as Map<String, dynamic>);
      });
}
