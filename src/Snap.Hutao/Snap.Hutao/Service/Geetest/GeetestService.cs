// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;
using Snap.Hutao.Web.Hutao.Geetest;
using Snap.Hutao.Web.Response;
using System.Text;

namespace Snap.Hutao.Service.Geetest;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGeetestService))]
internal sealed partial class GeetestService : IGeetestService
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly CustomGeetestClient customGeetestClient;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly CardClient cardClient;

    public async ValueTask<GeetestData?> TryVerifyGtChallengeAsync(string gt, string challenge, bool isOversea, CancellationToken token = default)
    {
        GeetestResponse response = await customGeetestClient.VerifyAsync(gt, challenge, token).ConfigureAwait(false);

        if (response is { Code: 0, Data: { } data })
        {
            return data;
        }

        string? result = await PrivateVerifyByWebViewAsync(gt, challenge, isOversea, token).ConfigureAwait(false);
        if (string.IsNullOrEmpty(result))
        {
            return default;
        }

        GeetestWebResponse? webResponse = JsonSerializer.Deserialize<GeetestWebResponse>(result);
        ArgumentNullException.ThrowIfNull(webResponse);

        return new GeetestData()
        {
            Gt = gt,
            Challenge = webResponse.Challenge,
            Validate = webResponse.Validate,
        };
    }

    public async ValueTask<string?> TryVerifyXrpcChallengeAsync(Model.Entity.User user, CardVerifiationHeaders headers, CancellationToken token = default)
    {
        Response<GeetestVerification> registrationResponse = await cardClient.CreateVerificationAsync(user, headers, token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(registrationResponse, infoBarService, out GeetestVerification? registration))
        {
            return default;
        }

        if (await TryVerifyGtChallengeAsync(registration.Gt, registration.Challenge, user.IsOversea, token).ConfigureAwait(false) is not { } data)
        {
            return default;
        }

        Response<VerificationResult> verifyResponse = await cardClient.VerifyVerificationAsync(user, headers, registration.Challenge, data.Validate, token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(verifyResponse, infoBarService, out VerificationResult? result))
        {
            return default;
        }

        return result.Challenge;
    }

    public async ValueTask<bool> TryVerifyAigisSessionAsync(IAigisProvider provider, string? rawSession, bool isOversea, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(rawSession))
        {
            return false;
        }

        AigisSession? session = JsonSerializer.Deserialize<AigisSession>(rawSession);
        ArgumentNullException.ThrowIfNull(session);

        GeetestVerification? verification = JsonSerializer.Deserialize<GeetestVerification>(session.Data);
        ArgumentNullException.ThrowIfNull(verification);

        if (await TryVerifyGtChallengeAsync(verification.Gt, verification.Challenge, isOversea, token).ConfigureAwait(false) is not { } data)
        {
            // Custom Geetest failed and user closed the window without completing the verification
            return false;
        }

        GeetestWebResponse result = new(data.Challenge, data.Validate);

        provider.Aigis = $"{session.SessionId};{Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result)))}";
        return true;
    }

    private async ValueTask<string?> PrivateVerifyByWebViewAsync(string gt, string challenge, bool isOversea, CancellationToken token)
    {
        await taskContext.SwitchToMainThreadAsync();
        GeetestWebView2ContentProvider contentProvider = new(gt, challenge, isOversea);
        ShowWebView2WindowAction.Show(contentProvider, currentXamlWindowReference.GetXamlRoot());

        await taskContext.SwitchToBackgroundAsync();
        return await contentProvider.GetResultAsync().ConfigureAwait(false);
    }
}