/// Mirrors `WatchLog.Application.Auth.AuthResult`.
class AuthResult {
  final String userId;
  final String accessToken;
  final DateTime accessTokenExpiresAt;
  final String refreshToken;

  AuthResult({
    required this.userId,
    required this.accessToken,
    required this.accessTokenExpiresAt,
    required this.refreshToken,
  });

  factory AuthResult.fromJson(Map<String, dynamic> json) => AuthResult(
        userId: json['userId'] as String,
        accessToken: json['accessToken'] as String,
        accessTokenExpiresAt: DateTime.parse(json['accessTokenExpiresAt'] as String),
        refreshToken: json['refreshToken'] as String,
      );
}
