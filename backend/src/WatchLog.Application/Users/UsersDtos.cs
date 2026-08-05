namespace WatchLog.Application.Users;

public record UserProfileDto(
    Guid Id,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    string Locale,
    int ThemePreference,
    bool IsPrivate,
    DateTimeOffset CreatedAt);

public record MeDto(
    Guid Id,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    string Locale,
    int ThemePreference,
    bool IsPrivate,
    DateTimeOffset CreatedAt,
    int FollowerCount,
    int FollowingCount);

public record UpdateProfileRequest(string? DisplayName, string? AvatarUrl, string? Bio, string? Locale, int? ThemePreference, bool? IsPrivate);
