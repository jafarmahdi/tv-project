using System.Text;
using System.Threading.RateLimiting;
using AspNet.Security.OAuth.Apple;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using WatchLog.Api.Middleware;
using WatchLog.Api.Realtime;
using WatchLog.Api.Security;
using WatchLog.Application;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Infrastructure;
using WatchLog.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// ---- Application + Infrastructure (Domain has no DI needs) ----
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ---- MVC + validation ----
builder.Services.AddScoped<ValidationFilter>();
builder.Services.AddControllers(options => options.Filters.AddService<ValidationFilter>());

// ---- Api-layer implementations of Application seams ----
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<INotificationPublisher, SignalRNotificationPublisher>();

// ---- Auth: JWT bearer (primary API auth) + external OAuth providers (Google/Microsoft/Apple/Facebook) ----
var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = "External";
});

// Registered with no inline options: `TokenValidationParameters` is filled in below via
// `AddOptions<JwtBearerOptions>().Configure<IOptions<JwtOptions>>(...)`, which resolves
// `IOptions<JwtOptions>` from DI *lazily* (the same path `TokenService` uses to issue tokens).
// Reading `builder.Configuration` directly here — before `builder.Build()` — would race any
// configuration source added later in the pipeline (e.g. `WebApplicationFactory`'s test overrides,
// or a reloadable secrets provider), silently validating tokens against a stale/wrong key.
authBuilder.AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearerOptions, jwtOptions) =>
    {
        var jwt = jwtOptions.Value;
        bearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                string.IsNullOrWhiteSpace(jwt.SigningKey) ? "development-only-signing-key-change-me-32-chars-min" : jwt.SigningKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        // SignalR sends the JWT via a query string param (browsers can't set ws headers), not the Authorization header.
        bearerOptions.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Temporary cookie used only to complete the external-provider OAuth handshake before we issue our own JWT.
authBuilder.AddCookie("External");

var googleClientId = builder.Configuration["Auth:Google:ClientId"];
if (!string.IsNullOrEmpty(googleClientId))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"]!;
        options.SignInScheme = "External";
        options.CallbackPath = "/api/v1/auth/external/google/callback";
    });
}

var msClientId = builder.Configuration["Auth:Microsoft:ClientId"];
if (!string.IsNullOrEmpty(msClientId))
{
    authBuilder.AddMicrosoftAccount(options =>
    {
        options.ClientId = msClientId;
        options.ClientSecret = builder.Configuration["Auth:Microsoft:ClientSecret"]!;
        options.SignInScheme = "External";
        options.CallbackPath = "/api/v1/auth/external/microsoft/callback";
    });
}

var fbAppId = builder.Configuration["Auth:Facebook:AppId"];
if (!string.IsNullOrEmpty(fbAppId))
{
    authBuilder.AddFacebook(options =>
    {
        options.AppId = fbAppId;
        options.AppSecret = builder.Configuration["Auth:Facebook:AppSecret"]!;
        options.SignInScheme = "External";
        options.CallbackPath = "/api/v1/auth/external/facebook/callback";
    });
}

var appleClientId = builder.Configuration["Auth:Apple:ClientId"];
if (!string.IsNullOrEmpty(appleClientId))
{
    authBuilder.AddApple(options =>
    {
        options.ClientId = appleClientId;
        options.TeamId = builder.Configuration["Auth:Apple:TeamId"]!;
        options.KeyId = builder.Configuration["Auth:Apple:KeyId"]!;
        options.PrivateKey = (_, _) =>
            Task.FromResult<ReadOnlyMemory<char>>((builder.Configuration["Auth:Apple:PrivateKey"] ?? string.Empty).AsMemory());
        options.SignInScheme = "External";
        options.CallbackPath = "/api/v1/auth/external/apple/callback";
    });
}

builder.Services.AddAuthorization();

// ---- Rate limiting (auth endpoints get a stricter fixed window; everything else a generous global one) ----
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 200, Window = TimeSpan.FromMinutes(1) }));

    options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
});

// ---- SignalR ----
builder.Services.AddSignalR();

// ---- Swagger / OpenAPI ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "WatchLog API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste a JWT access token (no 'Bearer ' prefix needed)."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            []
        }
    });
});

// ---- Health checks ----
// Connection strings are resolved lazily from DI-provided `IConfiguration` at check time (same
// reasoning as the JWT options above) rather than read once from `builder.Configuration` here.
builder.Services.AddHealthChecks()
    .AddNpgSql(sp => sp.GetRequiredService<IConfiguration>().GetConnectionString("Default")
        ?? "Host=localhost;Database=watchlog;Username=watchlog;Password=watchlog")
    .AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>());

// ---- CORS (Flutter web + admin dashboard dev servers) ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy => policy
        .SetIsOriginAllowed(_ => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Default");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationsHub>("/hubs/notifications");
app.MapHealthChecks("/health");

app.Run();

// Exposed for WebApplicationFactory<Program> in integration tests.
public partial class Program;
