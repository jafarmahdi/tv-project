using WatchLog.Domain.Enums;

namespace WatchLog.Application.Ratings;

public interface IRatingService
{
    Task RateAsync(Guid userId, RateRequest request, CancellationToken ct = default);
    Task<RatingSummaryDto> GetSummaryAsync(Guid? userId, TargetType targetType, Guid targetId, CancellationToken ct = default);
}
