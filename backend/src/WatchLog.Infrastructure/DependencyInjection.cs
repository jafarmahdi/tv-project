using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Common;
using WatchLog.Infrastructure.Caching;
using WatchLog.Infrastructure.ExternalServices;
using WatchLog.Infrastructure.Identity;
using WatchLog.Infrastructure.Persistence;
using WatchLog.Infrastructure.Security;

namespace WatchLog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WatchLogDbContext>((sp, options) =>
            options.UseNpgsql(sp.GetRequiredService<IConfiguration>().GetConnectionString("Default")));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<WatchLogDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis") ?? "localhost:6379"));

        services.AddOptions<TmdbOptions>().BindConfiguration(TmdbOptions.SectionName);
        services.AddHttpClient<ITmdbClient, TmdbClient>((sp, client) =>
        {
            var baseUrl = sp.GetRequiredService<IConfiguration>().GetSection(TmdbOptions.SectionName)["BaseUrl"]
                ?? "https://api.themoviedb.org/3";
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName);

        services.AddFido2(configuration.GetSection("Fido2"));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasskeyService, PasskeyService>();

        return services;
    }
}
