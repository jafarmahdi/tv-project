using FluentAssertions;
using WatchLog.Application.Stats;
using WatchLog.Application.Tests.TestSupport;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Tests.Stats;

public class StatsServiceTests
{
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly StatsService _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public StatsServiceTests()
    {
        _sut = new StatsService(_unitOfWork);
    }

    private (Episode Episode, Genre Genre) BuildWatchedEpisodeFixture(string genreName, int runtimeMinutes)
    {
        var genre = new Genre { Name = genreName };
        var series = new Series { Title = "Test Series" };
        series.Genres.Add(new SeriesGenre { Series = series, Genre = genre });
        var season = new Season { Series = series, SeasonNumber = 1 };
        var episode = new Episode { Season = season, EpisodeNumber = 1, RuntimeMinutes = runtimeMinutes };
        return (episode, genre);
    }

    [Fact]
    public async Task GetUserStatsAsync_CountsWatchedEpisodesAndMoviesAndSumsRuntime()
    {
        var (episode, _) = BuildWatchedEpisodeFixture("Drama", 45);
        _unitOfWork.Seed(new EpisodeProgress
        {
            UserId = _userId, EpisodeId = episode.Id, Episode = episode,
            Status = EpisodeWatchStatus.Watched, WatchedAt = DateTimeOffset.UtcNow
        });

        var movieGenre = new Genre { Name = "Action" };
        var movie = new Movie { Title = "Test Movie", RuntimeMinutes = 120 };
        movie.Genres.Add(new MovieGenre { Movie = movie, Genre = movieGenre });
        _unitOfWork.Seed(new MovieWatch
        {
            UserId = _userId, MovieId = movie.Id, Movie = movie,
            IsWatched = true, WatchedAt = DateTimeOffset.UtcNow
        });

        var stats = await _sut.GetUserStatsAsync(_userId);

        stats.TotalEpisodesWatched.Should().Be(1);
        stats.TotalMoviesWatched.Should().Be(1);
        stats.TotalWatchTimeMinutes.Should().Be(45 + 120);
    }

    [Fact]
    public async Task GetUserStatsAsync_RanksFavoriteGenresByWatchCount()
    {
        var (drama1, _) = BuildWatchedEpisodeFixture("Drama", 40);
        var (drama2, _) = BuildWatchedEpisodeFixture("Drama", 40);
        var (comedy1, _) = BuildWatchedEpisodeFixture("Comedy", 25);

        foreach (var episode in new[] { drama1, drama2, comedy1 })
        {
            _unitOfWork.Seed(new EpisodeProgress
            {
                UserId = _userId, EpisodeId = episode.Id, Episode = episode,
                Status = EpisodeWatchStatus.Watched, WatchedAt = DateTimeOffset.UtcNow
            });
        }

        var stats = await _sut.GetUserStatsAsync(_userId);

        stats.FavoriteGenres.Should().NotBeEmpty();
        stats.FavoriteGenres[0].Genre.Should().Be("Drama");
        stats.FavoriteGenres[0].Count.Should().Be(2);
    }

    [Fact]
    public async Task GetUserStatsAsync_IgnoresUnwatchedAndOtherUsersProgress()
    {
        var (unwatchedEpisode, _) = BuildWatchedEpisodeFixture("Drama", 40);
        _unitOfWork.Seed(new EpisodeProgress
        {
            UserId = _userId, EpisodeId = unwatchedEpisode.Id, Episode = unwatchedEpisode,
            Status = EpisodeWatchStatus.Unwatched
        });

        var (otherUsersEpisode, _) = BuildWatchedEpisodeFixture("Comedy", 30);
        _unitOfWork.Seed(new EpisodeProgress
        {
            UserId = Guid.NewGuid(), EpisodeId = otherUsersEpisode.Id, Episode = otherUsersEpisode,
            Status = EpisodeWatchStatus.Watched, WatchedAt = DateTimeOffset.UtcNow
        });

        var stats = await _sut.GetUserStatsAsync(_userId);

        stats.TotalEpisodesWatched.Should().Be(0);
    }
}
