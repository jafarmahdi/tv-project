using System.Text.Json;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;
using WatchLog.Infrastructure.Identity;
using WatchLog.Infrastructure.Persistence;

namespace WatchLog.Infrastructure.Security;

/// <summary>
/// WebAuthn/passkey ceremonies via Fido2NetLib. Stateless by design — the challenge options are
/// serialized to JSON and round-tripped through the client (mobile/web) instead of relying on
/// server-side session state, which keeps the API horizontally scalable.
/// </summary>
public class PasskeyService(IFido2 fido2, WatchLogDbContext dbContext, UserManager<ApplicationUser> userManager) : IPasskeyService
{
    public async Task<string> BeginRegistrationAsync(Guid userId, string username, string displayName, CancellationToken ct = default)
    {
        var existingCredentials = await dbContext.PasskeyCredentials
            .Where(c => c.UserId == userId)
            .Select(c => new PublicKeyCredentialDescriptor(c.CredentialId))
            .ToListAsync(ct);

        var fido2User = new Fido2User { Id = userId.ToByteArray(), Name = username, DisplayName = displayName };

        var options = fido2.RequestNewCredential(new RequestNewCredentialParams
        {
            User = fido2User,
            ExcludeCredentials = existingCredentials,
            AuthenticatorSelection = AuthenticatorSelection.Default,
            AttestationPreference = AttestationConveyancePreference.None
        });

        return options.ToJson();
    }

    public async Task CompleteRegistrationAsync(Guid userId, string attestationResponseJson, string originalOptionsJson,
        string? deviceName, CancellationToken ct = default)
    {
        var originalOptions = CredentialCreateOptions.FromJson(originalOptionsJson);
        var attestationResponse = JsonSerializer.Deserialize<AuthenticatorAttestationRawResponse>(attestationResponseJson)
            ?? throw new InvalidOperationException("Invalid attestation response payload.");

        var result = await fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
        {
            AttestationResponse = attestationResponse,
            OriginalOptions = originalOptions,
            IsCredentialIdUniqueToUserCallback = async (p, cbCt) =>
                !await dbContext.PasskeyCredentials.AnyAsync(c => c.CredentialId == p.CredentialId, cbCt)
        }, ct);

        dbContext.PasskeyCredentials.Add(new PasskeyCredential
        {
            UserId = userId,
            CredentialId = result.Id,
            PublicKey = result.PublicKey,
            SignCount = result.SignCount,
            AaGuid = result.AaGuid,
            DeviceName = deviceName
        });
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<string> BeginAssertionAsync(string? username, CancellationToken ct = default)
    {
        List<PublicKeyCredentialDescriptor> allowedCredentials = [];

        if (username is not null)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user is not null)
            {
                allowedCredentials = await dbContext.PasskeyCredentials
                    .Where(c => c.UserId == user.Id)
                    .Select(c => new PublicKeyCredentialDescriptor(c.CredentialId))
                    .ToListAsync(ct);
            }
        }

        var options = fido2.GetAssertionOptions(new GetAssertionOptionsParams
        {
            AllowedCredentials = allowedCredentials,
            UserVerification = UserVerificationRequirement.Preferred
        });

        return options.ToJson();
    }

    public async Task<Guid> CompleteAssertionAsync(string assertionResponseJson, string originalOptionsJson, CancellationToken ct = default)
    {
        var originalOptions = AssertionOptions.FromJson(originalOptionsJson);
        var assertionResponse = JsonSerializer.Deserialize<AuthenticatorAssertionRawResponse>(assertionResponseJson)
            ?? throw new InvalidOperationException("Invalid assertion response payload.");

        var credential = await dbContext.PasskeyCredentials
            .FirstOrDefaultAsync(c => c.CredentialId == assertionResponse.RawId, ct)
            ?? throw new UnauthorizedAccessException("This passkey is not registered.");

        var result = await fido2.MakeAssertionAsync(new MakeAssertionParams
        {
            AssertionResponse = assertionResponse,
            OriginalOptions = originalOptions,
            StoredPublicKey = credential.PublicKey,
            StoredSignatureCounter = credential.SignCount,
            IsUserHandleOwnerOfCredentialIdCallback = async (p, cbCt) =>
                await dbContext.PasskeyCredentials.AnyAsync(
                    c => c.CredentialId == p.CredentialId && c.UserId == new Guid(p.UserHandle), cbCt)
        }, ct);

        credential.SignCount = result.SignCount;
        await dbContext.SaveChangesAsync(ct);

        return credential.UserId;
    }
}
