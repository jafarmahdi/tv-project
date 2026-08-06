using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WatchLog.Infrastructure.Identity;

/// <summary>
/// Startup step: if <c>Admin:InitialAdminEmail</c> is configured and a user with that email
/// already exists, ensures they're in the "Admin" role. The role itself always exists (seeded
/// via migration — see <c>SeedData.Roles</c>); only the user→role assignment is dynamic, since
/// it depends on runtime config rather than something a migration can bake in. Safe to run on
/// every startup: it's a no-op once the user already holds the role.
/// </summary>
public static class IdentitySeeder
{
    public static async Task EnsureInitialAdminAsync(IServiceProvider services, IConfiguration configuration)
    {
        var initialAdminEmail = configuration["Admin:InitialAdminEmail"];
        if (string.IsNullOrWhiteSpace(initialAdminEmail)) return;

        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(initialAdminEmail);
        if (user is null) return;

        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
