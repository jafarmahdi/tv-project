using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Infrastructure.Identity;

public class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<IdentityCreateResult> CreateUserAsync(string email, string password, string displayName, string locale)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName,
            Locale = locale
        };

        var result = await userManager.CreateAsync(user, password);
        return result.Succeeded
            ? new IdentityCreateResult(true, user.Id, [])
            : new IdentityCreateResult(false, null, result.Errors.Select(e => e.Description).ToList());
    }

    public async Task<Guid?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return null;

        return await userManager.CheckPasswordAsync(user, password) ? user.Id : null;
    }

    public async Task<Guid> FindOrCreateExternalUserAsync(string provider, string providerKey, string email, string displayName)
    {
        var user = await userManager.FindByLoginAsync(provider, providerKey);
        if (user is not null) return user.Id;

        user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, DisplayName = displayName, EmailConfirmed = true };
            var randomPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var createResult = await userManager.CreateAsync(user, randomPassword);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(" ", createResult.Errors.Select(e => e.Description)));
            }
        }

        await userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));
        return user.Id;
    }

    public async Task<UserAccountDto?> GetUserAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        return user is null ? null : ToDto(user);
    }

    public async Task<IReadOnlyDictionary<Guid, UserAccountDto>> GetUsersAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new Dictionary<Guid, UserAccountDto>();

        var users = await userManager.Users.Where(u => idList.Contains(u.Id)).ToListAsync();
        return users.ToDictionary(u => u.Id, ToDto);
    }

    public async Task<bool> UpdateProfileAsync(Guid id, string? displayName, string? avatarUrl, string? bio,
        string? locale, int? themePreference, bool? isPrivate)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return false;

        if (displayName is not null) user.DisplayName = displayName;
        if (avatarUrl is not null) user.AvatarUrl = avatarUrl;
        if (bio is not null) user.Bio = bio;
        if (locale is not null) user.Locale = locale;
        if (themePreference is not null) user.ThemePreference = (Domain.Enums.ThemePreference)themePreference.Value;
        if (isPrivate is not null) user.IsPrivate = isPrivate.Value;

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return [];
        return (await userManager.GetRolesAsync(user)).ToList();
    }

    private static UserAccountDto ToDto(ApplicationUser user) => new(
        user.Id, user.Email!, user.DisplayName, user.AvatarUrl, user.Bio, user.Locale,
        (int)user.ThemePreference, user.IsPrivate, user.CreatedAt);
}
