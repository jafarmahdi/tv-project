namespace WatchLog.Application.Common.Interfaces;

/// <summary>
/// Thin seam over ASP.NET Core Identity so `WatchLog.Application` never references
/// `UserManager`/`SignInManager` directly. Implemented in `WatchLog.Infrastructure`.
/// </summary>
public interface IIdentityService
{
    Task<IdentityCreateResult> CreateUserAsync(string email, string password, string displayName, string locale);
    Task<Guid?> ValidateCredentialsAsync(string email, string password);
    Task<Guid> FindOrCreateExternalUserAsync(string provider, string providerKey, string email, string displayName);
    Task<UserAccountDto?> GetUserAsync(Guid id);

    /// <summary>Batch lookup, used when rendering feeds/comment lists that reference many distinct users.</summary>
    Task<IReadOnlyDictionary<Guid, UserAccountDto>> GetUsersAsync(IEnumerable<Guid> ids);
    Task<bool> UpdateProfileAsync(Guid id, string? displayName, string? avatarUrl, string? bio, string? locale, int? themePreference, bool? isPrivate);
    Task<IReadOnlyList<string>> GetRolesAsync(Guid id);
}

public record IdentityCreateResult(bool Succeeded, Guid? UserId, IReadOnlyList<string> Errors);

public record UserAccountDto(
    Guid Id,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    string Locale,
    int ThemePreference,
    bool IsPrivate,
    DateTimeOffset CreatedAt);
