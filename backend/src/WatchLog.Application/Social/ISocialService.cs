namespace WatchLog.Application.Social;

public interface ISocialService
{
    Task FollowAsync(Guid followerId, Guid targetUserId, CancellationToken ct = default);
    Task UnfollowAsync(Guid followerId, Guid targetUserId, CancellationToken ct = default);
    Task<IReadOnlyList<FollowSummaryDto>> GetFollowersAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<FollowSummaryDto>> GetFollowingAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Activity from the people the caller follows (plus their own), newest first.</summary>
    Task<IReadOnlyList<ActivityFeedItemDto>> GetFeedAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);

    Task<CommentDto> AddCommentAsync(Guid userId, AddCommentRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<CommentDto>> GetCommentsAsync(Domain.Enums.TargetType targetType, Guid targetId, CancellationToken ct = default);
    Task ToggleLikeAsync(Guid userId, ToggleLikeRequest request, CancellationToken ct = default);
}
