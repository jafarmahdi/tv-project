namespace WatchLog.Application.Auth;

public record RegisterRequest(string Email, string Password, string DisplayName, string Locale = "en");
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record ExternalLoginRequest(string Provider, string ProviderKey, string Email, string DisplayName);

public record AuthResult(Guid UserId, string AccessToken, DateTimeOffset AccessTokenExpiresAt, string RefreshToken);
