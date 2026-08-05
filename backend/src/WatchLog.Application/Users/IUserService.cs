namespace WatchLog.Application.Users;

public interface IUserService
{
    Task<MeDto> GetMeAsync(Guid userId, CancellationToken ct = default);
    Task<UserProfileDto> GetPublicProfileAsync(Guid targetUserId, CancellationToken ct = default);
    Task<MeDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default);
}
