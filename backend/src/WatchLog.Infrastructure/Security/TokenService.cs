using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Infrastructure.Security;

public class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_options.RefreshTokenDays);

    public (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public (string RawToken, string Hash) GenerateRefreshToken()
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return (raw, HashRefreshToken(raw));
    }

    public string HashRefreshToken(string rawToken) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        try
        {
            return new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }
}
