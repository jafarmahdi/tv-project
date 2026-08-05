using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WatchLog.Application.Achievements;
using WatchLog.Application.Ai;
using WatchLog.Application.Auth;
using WatchLog.Application.Catalog;
using WatchLog.Application.Collections;
using WatchLog.Application.Devices;
using WatchLog.Application.Lists;
using WatchLog.Application.Notifications;
using WatchLog.Application.Ratings;
using WatchLog.Application.Social;
using WatchLog.Application.Stats;
using WatchLog.Application.Tracking;
using WatchLog.Application.Users;

namespace WatchLog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IEpisodeTrackingService, EpisodeTrackingService>();
        services.AddScoped<IMovieTrackingService, MovieTrackingService>();
        services.AddScoped<IListService, ListService>();
        services.AddScoped<IStatsService, StatsService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISocialService, SocialService>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<IAiAssistantService, AiAssistantService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IRatingService, RatingService>();

        return services;
    }
}
