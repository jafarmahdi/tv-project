using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using WatchLog.Infrastructure.Persistence;

namespace WatchLog.Api.IntegrationTests;

/// <summary>
/// Boots the real API against real, disposable Postgres + Redis containers (via Testcontainers) —
/// proving the whole stack actually wires together and talks to real infra, not just that it compiles.
/// Requires a running Docker daemon.
/// </summary>
public class WatchLogApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("watchlog_test")
        .WithUsername("watchlog")
        .WithPassword("watchlog")
        .Build();

    private readonly RedisContainer _redis = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_postgres.StartAsync(), _redis.StartAsync());
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(_postgres.DisposeAsync().AsTask(), _redis.DisposeAsync().AsTask());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _postgres.GetConnectionString(),
                ["ConnectionStrings:Redis"] = _redis.GetConnectionString(),
                ["Jwt:SigningKey"] = "integration-test-signing-key-not-for-production-32chars"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Apply migrations against the disposable container instead of relying on `dotnet ef database update`.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<WatchLogDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<WatchLogDbContext>(options => options.UseNpgsql(_postgres.GetConnectionString()));
        });
    }

    public async Task MigrateAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WatchLogDbContext>();
        await db.Database.MigrateAsync();
    }
}
