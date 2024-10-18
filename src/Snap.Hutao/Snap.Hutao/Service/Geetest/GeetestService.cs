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
    private readonly HomaGeetestClient homaGeetestClient;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly CardClient cardClient;

    public async ValueTask<GeetestData?> TryVerifyAsync(string gt, string challenge, CancellationToken token = default)
    {
        GeetestResponse response = await homaGeetestClient.VerifyAsync(gt, challenge, token).ConfigureAwait(false);

        if (response is { Code: 0, Data: { } data })
        {
            return data;
        }

        string? result = await VerifyByWebViewAsync(gt, challenge, false, token).ConfigureAwait(false);
        if (string.IsNullOrEmpty(result))
        {
            return default;
        }

        GeetestWebResponse? webResponse = JsonSerializer.Deserialize<GeetestWebResponse>(result);
        ArgumentNullException.ThrowIfNull(webResponse);
        GeetestData webData = new()
        {
            Gt = gt,
            Challenge = webResponse.Challenge,
            Validate = webResponse.Validate,
        };
        return webData;
    }

    public async ValueTask<string?> TryValidateXrpcChallengeAsync(Model.Entity.User user, CardVerifiationHeaders headers, CancellationToken token = default)
    {
        Response<VerificationRegistration> registrationResponse = await cardClient.CreateVerificationAsync(user, headers, token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(registrationResponse, infoBarService, out VerificationRegistration? registration))
        {
            return default;
        }

        if (await TryVerifyAsync(registration.Gt, registration.Challenge, token).ConfigureAwait(false) is not { } data)
        {
            return default;
        }

        Response<VerificationResult> verifyResponse = await cardClient.VerifyVerificationAsync(user, headers, registration.Challenge, data.Validate, token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(verifyResponse, infoBarService, out VerificationResult? result))
        {
            return default;
        }

        if (result.Challenge is not null)
        {
            return result.Challenge;
        }

        return default;
    }

    public async ValueTask<bool> TryResolveAigisAsync(IAigisProvider provider, string? rawSession, bool isOversea, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(rawSession))
        {
            return false;
        }

        AigisObject? session = JsonSerializer.Deserialize<AigisObject>(rawSession);
        ArgumentNullException.ThrowIfNull(session);
        AigisData? sessionData = JsonSerializer.Deserialize<AigisData>(session.Data);
        ArgumentNullException.ThrowIfNull(sessionData);

        string? result = await VerifyByWebViewAsync(sessionData.GT, sessionData.Challenge, isOversea, token).ConfigureAwait(false);

        if (string.IsNullOrEmpty(result))
        {
            // User closed the window without completing the verification
            return false;
        }

        provider.Aigis = $"{session.SessionId};{Convert.ToBase64String(Encoding.UTF8.GetBytes(result))}";
        return true;
    }

    private async ValueTask<string?> VerifyByWebViewAsync(string gt, string challenge, bool isOversea, CancellationToken token)
    {
        await taskContext.SwitchToMainThreadAsync();
        GeetestWebView2ContentProvider contentProvider = new(gt, challenge, isOversea);

        new ShowWebView2WindowAction
        {
            ContentProvider = contentProvider,
        }.ShowAt(currentXamlWindowReference.GetXamlRoot());

        await taskContext.SwitchToBackgroundAsync();
        return await contentProvider.GetResultAsync().ConfigureAwait(false);
    }
}