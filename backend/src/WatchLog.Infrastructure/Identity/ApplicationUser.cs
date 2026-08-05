using Microsoft.AspNetCore.Identity;
using WatchLog.Domain.Enums;

namespace WatchLog.Infrastructure.Identity;

/// <summary>
/// ASP.NET Core Identity user, extended with the profile fields WatchLog needs. Kept in
/// Infrastructure (not Domain) because it depends on `IdentityUser&lt;Guid&gt;` — Domain entities
/// only ever hold a plain `Guid UserId`, never a reference to this type.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = default!;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string Locale { get; set; } = "en";
    public ThemePreference ThemePreference { get; set; } = ThemePreference.System;
    public bool IsPrivate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
}
