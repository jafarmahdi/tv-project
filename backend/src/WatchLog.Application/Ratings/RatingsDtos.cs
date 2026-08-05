using WatchLog.Domain.Enums;

namespace WatchLog.Application.Ratings;

public record RateRequest(TargetType TargetType, Guid TargetId, int Score);
public record RatingSummaryDto(double Average, int Count, int? MyScore);
