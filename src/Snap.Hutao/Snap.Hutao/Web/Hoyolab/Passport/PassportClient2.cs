// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Globalization;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc2)]
internal sealed partial class PassportClient2
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<PassportClient2> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<UserInfoWrapper>> VerifyLtokenAsync(User user, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AccountVerifyLtoken)
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .PostJson(new Timestamp());

        Response<UserInfoWrapper>? resp = await builder
            .TryCatchSendAsync<Response<UserInfoWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<LoginResult>> LoginBySTokenAsync(Cookie stokenV1, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AccountGetSTokenByOldToken)
            .SetHeader("Cookie", stokenV1.ToString())
            .Post();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.PROD, true).ConfigureAwait(false);

        Response<LoginResult>? resp = await builder
            .TryCatchSendAsync<Response<LoginResult>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<LoginResult>> GetSTokenByGameTokenAsync(UidGameToken account, CancellationToken token = default)
    {
        AccountIdGameToken data = new()
        {
            AccountId = int.Parse(account.Uid, CultureInfo.InvariantCulture),
            GameToken = account.GameToken,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AccountGetSTokenByGameToken)
            .SetHeader("x-rpc-device_id", HoyolabOptions.DeviceId40)
            .PostJson(data);

        Response<LoginResult>? resp = await builder
            .TryCatchSendAsync<Response<LoginResult>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    private class Timestamp
    {
        [JsonPropertyName("t")]
        public long Time { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}