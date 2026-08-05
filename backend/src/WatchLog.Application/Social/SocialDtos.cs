using WatchLog.Domain.Enums;

namespace WatchLog.Application.Social;

public record ActivityFeedItemDto(Guid Id, Guid UserId, string UserDisplayName, string? UserAvatarUrl,
    ActivityType Type, TargetType? TargetType, Guid? TargetId, string? MetadataJson, DateTimeOffset CreatedAt);

public record CommentDto(Guid Id, Guid UserId, string UserDisplayName, string? UserAvatarUrl, string Body,
    Guid? ParentCommentId, DateTimeOffset CreatedAt, int LikeCount);

public record AddCommentRequest(TargetType TargetType, Guid TargetId, string Body, Guid? ParentCommentId = null);
public record ToggleLikeRequest(TargetType TargetType, Guid TargetId);
public record FollowSummaryDto(Guid UserId, string DisplayName, string? AvatarUrl, bool FollowsYouBack);
