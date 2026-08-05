using WatchLog.Domain.Common;
using WatchLog.Domain.Enums;

namespace WatchLog.Domain.Entities;

/// <summary>
/// A user's list. Every user gets the six built-in <see cref="ListType"/> lists on registration;
/// <see cref="ListType.Custom"/> lists are user-created and use <see cref="Name"/>.
/// </summary>
public class UserList : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public ListType Type { get; set; }
    public bool IsPublic { get; set; } = true;

    public ICollection<ListItem> Items { get; set; } = new List<ListItem>();
}

public class ListItem : Entity
{
    public Guid ListId { get; set; }
    public UserList List { get; set; } = default!;
    public Guid? MovieId { get; set; }
    public Movie? Movie { get; set; }
    public Guid? SeriesId { get; set; }
    public Series? Series { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>Per-user, per-episode watch progress — the core of episode tracking.</summary>
public class EpisodeProgress : Entity
{
    public Guid UserId { get; set; }
    public Guid EpisodeId { get; set; }
    public Episode Episode { get; set; } = default!;
    public EpisodeWatchStatus Status { get; set; } = EpisodeWatchStatus.Unwatched;
    public bool IsFavorite { get; set; }
    public DateTimeOffset? WatchedAt { get; set; }
}

/// <summary>Per-user movie watch state (movies don't have episodes, so this is simpler than <see cref="EpisodeProgress"/>).</summary>
public class MovieWatch : Entity
{
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public Movie Movie { get; set; } = default!;
    public bool IsWatched { get; set; }
    public bool IsFavorite { get; set; }
    public DateTimeOffset? WatchedAt { get; set; }
}
