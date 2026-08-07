using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;

namespace WatchLog.Application.Users;

public class UserService(IIdentityService identityService, IUnitOfWork unitOfWork) : IUserService
{
    public async Task<MeDto> GetMeAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await identityService.GetUserAsync(userId) ?? throw new NotFoundException("User", userId);
        var roles = await identityService.GetRolesAsync(userId);
        var followerCount = await unitOfWork.Repository<Follow>().Query().CountAsync(f => f.FollowingId == userId, ct);
        var followingCount = await unitOfWork.Repository<Follow>().Query().CountAsync(f => f.FollowerId == userId, ct);

        return new MeDto(user.Id, user.Email, user.DisplayName, user.AvatarUrl, user.Bio, user.Locale,
            user.ThemePreference, user.IsPrivate, user.CreatedAt, followerCount, followingCount,
            roles.Contains("Admin", StringComparer.OrdinalIgnoreCase));
    }

    public async Task<UserProfileDto> GetPublicProfileAsync(Guid targetUserId, CancellationToken ct = default)
    {
        var user = await identityService.GetUserAsync(targetUserId) ?? throw new NotFoundException("User", targetUserId);
        return new UserProfileDto(user.Id, user.DisplayName, user.AvatarUrl, user.Bio, user.Locale,
            user.ThemePreference, user.IsPrivate, user.CreatedAt);
    }

    public async Task<MeDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var updated = await identityService.UpdateProfileAsync(
            userId, request.DisplayName, request.AvatarUrl, request.Bio, request.Locale,
            request.ThemePreference, request.IsPrivate);

        if (!updated) throw new NotFoundException("User", userId);

        return await GetMeAsync(userId, ct);
    }
}
