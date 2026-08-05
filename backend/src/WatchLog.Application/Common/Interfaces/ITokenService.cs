using System.Security.Claims;

namespace WatchLog.Application.Common.Interfaces;

/// <summary>Issues and validates JWT access tokens and opaque refresh tokens. Implemented in Infrastructure.</summary>
public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);

    /// <summary>Returns the raw refresh token to hand to the client, and its SHA-256 hash to persist.</summary>
    (string RawToken, string Hash) GenerateRefreshToken();

    string HashRefreshToken(string rawToken);
    ClaimsPrincipal? ValidateAccessToken(string token);

    /// <summary>How long a refresh token stays valid, driven by `Jwt:RefreshTokenDays` configuration.</summary>
    TimeSpan RefreshTokenLifetime { get; }
}
