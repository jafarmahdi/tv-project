namespace WatchLog.Infrastructure.Security;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "watchlog.local";
    public string Audience { get; set; } = "watchlog.clients";
    public string SigningKey { get; set; } = default!;
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 30;
}
