using FluentAssertions;
using Moq;
using WatchLog.Application.Achievements;
using WatchLog.Application.Catalog;
using WatchLog.Application.Tests.TestSupport;
using WatchLog.Application.Tracking;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Tests.Tracking;

public class EpisodeTrackingServiceTests
{
    private readonly Mock<ICatalogService> _catalog = new();
    private readonly Mock<IAchievementService> _achievements = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly EpisodeTrackingService _sut;
    private readonly Guid _episodeId = Guid.NewGuid();

    public EpisodeTrackingServiceTests()
    {
        _catalog.Setup(c => c.EnsureEpisodeCachedAsync(1396, 1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(_episodeId);
        _sut = new EpisodeTrackingService(_catalog.Object, _unitOfWork, _achievements.Object);
    }

    [Fact]
    public async Task MarkEpisodeAsync_AsWatched_CreatesProgressActivityEntryAndEvaluatesAchievements()
    {
        var userId = Guid.NewGuid();

        await _sut.MarkEpisodeAsync(userId, new MarkEpisodeRequest(1396, 1, 1, EpisodeWatchStatus.Watched));

        var progress = _unitOfWork.Repository<EpisodeProgress>().Query().Should().ContainSingle().Subject;
        progress.UserId.Should().Be(userId);
        progress.EpisodeId.Should().Be(_episodeId);
        progress.Status.Should().Be(EpisodeWatchStatus.Watched);
        progress.WatchedAt.Should().NotBeNull();

        _unitOfWork.Repository<ActivityFeedEntry>().Query()
            .Should().ContainSingle(a => a.UserId == userId && a.Type == ActivityType.WatchedEpisode);

        _achievements.Verify(a => a.EvaluateAndAwardAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkEpisodeAsync_AsSkipped_DoesNotPostActivityOrEvaluateAchievements()
    {
        var userId = Guid.NewGuid();

        await _sut.MarkEpisodeAsync(userId, new MarkEpisodeRequest(1396, 1, 1, EpisodeWatchStatus.Skipped));

        _unitOfWork.Repository<ActivityFeedEntry>().Query().Should().BeEmpty();
        _achievements.Verify(a => a.EvaluateAndAwardAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MarkEpisodeAsync_CalledTwice_UpdatesExistingProgressInsteadOfDuplicating()
    {
        var userId = Guid.NewGuid();

        await _sut.MarkEpisodeAsync(userId, new MarkEpisodeRequest(1396, 1, 1, EpisodeWatchStatus.Watched));
        await _sut.MarkEpisodeAsync(userId, new MarkEpisodeRequest(1396, 1, 1, EpisodeWatchStatus.Skipped));

        _unitOfWork.Repository<EpisodeProgress>().Query().Should().ContainSingle()
            .Which.Status.Should().Be(EpisodeWatchStatus.Skipped);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_SetsFavoriteWithoutChangingWatchStatus()
    {
        var userId = Guid.NewGuid();
        await _sut.MarkEpisodeAsync(userId, new MarkEpisodeRequest(1396, 1, 1, EpisodeWatchStatus.Watched));

        await _sut.ToggleFavoriteAsync(userId, new ToggleEpisodeFavoriteRequest(1396, 1, 1, true));

        var progress = _unitOfWork.Repository<EpisodeProgress>().Query().Should().ContainSingle().Subject;
        progress.IsFavorite.Should().BeTrue();
        progress.Status.Should().Be(EpisodeWatchStatus.Watched, "favoriting shouldn't reset watch progress");
    }
}
