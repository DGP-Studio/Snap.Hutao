// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Geetest;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HomaGeetestClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<HomaGeetestClient> logger;
    private readonly AppOptions appOptions;

    public async ValueTask<GeetestResponse> VerifyAsync(string gt, string challenge, CancellationToken token)
    {
        string template = appOptions.GeetestCustomCompositeUrl;

        if (string.IsNullOrEmpty(template))
        {
            return GeetestResponse.InternalFailure;
        }

        GeetestResponse? resp = await httpClient
            .TryCatchGetFromJsonAsync<GeetestResponse>(template.Format(gt, challenge), options, logger, token)
            .ConfigureAwait(false);

        if (resp is null)
        {
            return GeetestResponse.InternalFailure;
        }

        return resp;
    }
}
