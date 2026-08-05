namespace WatchLog.Application.Common.Interfaces;

/// <summary>Pushes real-time events to a user's connected clients. Implemented in the Api layer (SignalR `IHubContext`).</summary>
public interface INotificationPublisher
{
    Task PushNotificationAsync(Guid userId, object payload, CancellationToken ct = default);
    Task PushActivityAsync(Guid userId, object payload, CancellationToken ct = default);
}

/// <summary>Resolves the caller's identity from the current HTTP request. Implemented in the Api layer.</summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}

/// <summary>WebAuthn/passkey registration and assertion ceremonies (Fido2NetLib-backed). Implemented in Infrastructure.</summary>
public interface IPasskeyService
{
    Task<string> BeginRegistrationAsync(Guid userId, string username, string displayName, CancellationToken ct = default);
    Task CompleteRegistrationAsync(Guid userId, string attestationResponseJson, string originalOptionsJson, string? deviceName, CancellationToken ct = default);
    Task<string> BeginAssertionAsync(string? username, CancellationToken ct = default);
    Task<Guid> CompleteAssertionAsync(string assertionResponseJson, string originalOptionsJson, CancellationToken ct = default);
}
