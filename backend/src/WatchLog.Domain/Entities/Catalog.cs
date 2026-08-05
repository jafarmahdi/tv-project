using WatchLog.Domain.Common;
using WatchLog.Domain.Enums;

namespace WatchLog.Domain.Entities;

/// <summary>A TMDB genre, cached locally so we can query/filter without round-tripping to TMDB.</summary>
public class Genre : Entity
{
    public int TmdbId { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    public ICollection<SeriesGenre> SeriesGenres { get; set; } = new List<SeriesGenre>();
}

/// <summary>
/// A locally-cached copy of a TMDB movie. Refreshed on a TTL via <c>ITmdbClient</c> so lists,
/// stats and search can be served from Postgres/Redis instead of hammering TMDB on every request.
/// </summary>
public class Movie : AuditableEntity
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = default!;
    public string? OriginalTitle { get; set; }
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public int? RuntimeMinutes { get; set; }
    public double VoteAverage { get; set; }
    public double Popularity { get; set; }
    public string? TrailerYoutubeKey { get; set; }

    public ICollection<MovieGenre> Genres { get; set; } = new List<MovieGenre>();
}

public class MovieGenre
{
    public Guid MovieId { get; set; }
    public Movie Movie { get; set; } = default!;
    public Guid GenreId { get; set; }
    public Genre Genre { get; set; } = default!;
}

/// <summary>A locally-cached copy of a TMDB TV series, mirroring <see cref="Movie"/>.</summary>
public class Series : AuditableEntity
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = default!;
    public string? OriginalTitle { get; set; }
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public DateOnly? FirstAirDate { get; set; }
    public DateOnly? LastAirDate { get; set; }
    public SeriesStatus Status { get; set; }
    public double VoteAverage { get; set; }
    public double Popularity { get; set; }
    public string? TrailerYoutubeKey { get; set; }

    public ICollection<SeriesGenre> Genres { get; set; } = new List<SeriesGenre>();
    public ICollection<Season> Seasons { get; set; } = new List<Season>();
}

public class SeriesGenre
{
    public Guid SeriesId { get; set; }
    public Series Series { get; set; } = default!;
    public Guid GenreId { get; set; }
    public Genre Genre { get; set; } = default!;
}

public class Season : Entity
{
    public Guid SeriesId { get; set; }
    public Series Series { get; set; } = default!;
    public int SeasonNumber { get; set; }
    public string Name { get; set; } = default!;
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public DateOnly? AirDate { get; set; }

    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}

public class Episode : Entity
{
    public Guid SeasonId { get; set; }
    public Season Season { get; set; } = default!;
    public int EpisodeNumber { get; set; }
    public string Title { get; set; } = default!;
    public string? Overview { get; set; }
    public string? StillPath { get; set; }
    public DateOnly? AirDate { get; set; }
    public int? RuntimeMinutes { get; set; }
}
