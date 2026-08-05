using WatchLog.Domain.Common;
using WatchLog.Domain.Enums;

namespace WatchLog.Domain.Entities;

public class Achievement : Entity
{
    /// <summary>Stable machine key, e.g. "first-100-episodes" — referenced by the badge-evaluation job.</summary>
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? IconUrl { get; set; }
    public string CriteriaDescription { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}

public class UserAchievement : Entity
{
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public Achievement Achievement { get; set; } = default!;
    public DateTimeOffset EarnedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class Notification : Entity
{
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string? DataJson { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class Recommendation : Entity
{
    public Guid UserId { get; set; }
    public TargetType TargetType { get; set; }
    public Guid TargetId { get; set; }
    public double Score { get; set; }
    public string? Reason { get; set; }
    public RecommendationSource Source { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>One row per AI Assistant turn — powers "AI automatically learns user preferences".</summary>
public class AiHistoryEntry : Entity
{
    public Guid UserId { get; set; }
    public string Prompt { get; set; } = default!;
    public string Response { get; set; } = default!;
    public string? MetadataJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class Device : Entity
{
    public Guid UserId { get; set; }
    public DevicePlatform Platform { get; set; }
    public string? PushToken { get; set; }
    public string? DeviceName { get; set; }
    public DateTimeOffset LastSeenAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>A curated (e.g. "Marvel", "Oscar Winners") or user-created collection of movies/series.</summary>
public class Collection : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? PosterUrl { get; set; }
    public bool IsCurated { get; set; }
    public Guid? CreatedByUserId { get; set; }

    public ICollection<CollectionItem> Items { get; set; } = new List<CollectionItem>();
}

public class CollectionItem : Entity
{
    public Guid CollectionId { get; set; }
    public Collection Collection { get; set; } = default!;
    public Guid? MovieId { get; set; }
    public Movie? Movie { get; set; }
    public Guid? SeriesId { get; set; }
    public Series? Series { get; set; }
    public int SortOrder { get; set; }
}

public class RefreshToken : Entity
{
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = default!;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}

/// <summary>A stored WebAuthn/passkey credential for a user (Fido2NetLib-backed).</summary>
public class PasskeyCredential : Entity
{
    public Guid UserId { get; set; }
    public byte[] CredentialId { get; set; } = default!;
    public byte[] PublicKey { get; set; } = default!;
    public uint SignCount { get; set; }
    public Guid AaGuid { get; set; }
    public string? DeviceName { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
