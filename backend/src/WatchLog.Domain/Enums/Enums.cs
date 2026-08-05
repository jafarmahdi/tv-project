namespace WatchLog.Domain.Enums;

/// <summary>The six built-in list types every user gets, plus custom user-defined lists.</summary>
public enum ListType
{
    Watching = 0,
    Completed = 1,
    Planned = 2,
    OnHold = 3,
    Dropped = 4,
    Favorites = 5,
    Custom = 6
}

/// <summary>What a piece of user-generated content (rating/comment/like) is attached to.</summary>
public enum TargetType
{
    Movie = 0,
    Series = 1,
    Episode = 2,
    Comment = 3,
    UserList = 4,
    User = 5
}

/// <summary>Per-episode tracking state.</summary>
public enum EpisodeWatchStatus
{
    Unwatched = 0,
    Watched = 1,
    Skipped = 2
}

/// <summary>Series production status, mirrors TMDB's `status` field.</summary>
public enum SeriesStatus
{
    ReturningSeries = 0,
    Ended = 1,
    Cancelled = 2,
    InProduction = 3,
    Planned = 4
}

public enum NotificationType
{
    NewEpisode = 0,
    NewSeason = 1,
    UpcomingRelease = 2,
    FriendActivity = 3,
    Achievement = 4,
    System = 5
}

public enum RecommendationSource
{
    Algorithm = 0,
    Ai = 1,
    Curated = 2
}

public enum ActivityType
{
    WatchedEpisode = 0,
    WatchedMovie = 1,
    RatedItem = 2,
    CreatedList = 3,
    EarnedAchievement = 4,
    FollowedUser = 5,
    PostedComment = 6
}

public enum DevicePlatform
{
    Ios = 0,
    Android = 1,
    Web = 2,
    Windows = 3,
    MacOs = 4,
    Linux = 5
}

public enum ThemePreference
{
    System = 0,
    Light = 1,
    Dark = 2
}
