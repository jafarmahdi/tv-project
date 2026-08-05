using WatchLog.Domain.Common;
using WatchLog.Domain.Enums;

namespace WatchLog.Domain.Entities;

public class Rating : AuditableEntity
{
    public Guid UserId { get; set; }
    public TargetType TargetType { get; set; }
    public Guid TargetId { get; set; }

    /// <summary>1-10 scale, matching TMDB's convention so aggregates are directly comparable.</summary>
    public int Score { get; set; }
}

public class Comment : AuditableEntity
{
    public Guid UserId { get; set; }
    public TargetType TargetType { get; set; }
    public Guid TargetId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Body { get; set; } = default!;
    public bool IsDeleted { get; set; }
}

public class Like : Entity
{
    public Guid UserId { get; set; }
    public TargetType TargetType { get; set; }
    public Guid TargetId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class Follow : Entity
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>One row per social-feed-worthy action; fans out to followers' activity feeds.</summary>
public class ActivityFeedEntry : Entity
{
    public Guid UserId { get; set; }
    public ActivityType Type { get; set; }
    public TargetType? TargetType { get; set; }
    public Guid? TargetId { get; set; }

    /// <summary>Small, denormalized JSON payload (e.g. episode/show title) so the feed renders without N+1 lookups.</summary>
    public string? MetadataJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
