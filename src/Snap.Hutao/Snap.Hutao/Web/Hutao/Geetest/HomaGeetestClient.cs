// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Hutao.GachaLog;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Geetest;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HomaGeetestClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<HomaGeetestClient> logger;
    private readonly HutaoUserOptions hutaoUserOptions;

    public async ValueTask<GeetestResponse> VerifyAsync(string gt, string challenge, CancellationToken token)
    {
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        GeetestResponse? resp = await httpClient
            .TryCatchGetFromJsonAsync<GeetestResponse>(HutaoEndpoints.GeetestVerify(gt, challenge), options, logger, token)
            .ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(resp);
        return resp;
    }
}
