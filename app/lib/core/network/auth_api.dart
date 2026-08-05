import 'api_client.dart';
import '../models/auth_models.dart';

class AuthApi {
  final ApiClient _client;
  AuthApi(this._client);

  Future<AuthResult> register({required String email, required String password, required String displayName}) =>
      _client.guard(() async {
        final response = await _client.dio.post('/api/v1/auth/register', data: {
          'email': email,
          'password': password,
          'displayName': displayName,
        });
        return AuthResult.fromJson(response.data as Map<String, dynamic>);
      });

  Future<AuthResult> login({required String email, required String password}) => _client.guard(() async {
        final response = await _client.dio.post('/api/v1/auth/login', data: {'email': email, 'password': password});
        return AuthResult.fromJson(response.data as Map<String, dynamic>);
      });

  Future<void> logout(String refreshToken) => _client.guard(() async {
        await _client.dio.post('/api/v1/auth/logout', data: {'refreshToken': refreshToken});
      });
}
