// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Geetest;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HomaGeetestClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HomaGeetestClient> logger;
    private readonly AppOptions appOptions;
    private readonly HttpClient httpClient;

    public async ValueTask<GeetestResponse> VerifyAsync(string gt, string challenge, CancellationToken token)
    {
        string template = appOptions.GeetestCustomCompositeUrl;

        string url;
        try
        {
            url = template.Format(gt, challenge);
        }
        catch (FormatException)
        {
            return GeetestResponse.InternalFailure;
        }

        if (string.IsNullOrEmpty(template) || !Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            return GeetestResponse.InternalFailure;
        }

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(uri)
            .Get();

        GeetestResponse? resp = await builder
            .TryCatchSendAsync<GeetestResponse>(httpClient, logger, token)
            .ConfigureAwait(false);

        if (resp is null)
        {
            return GeetestResponse.InternalFailure;
        }

        return resp;
    }
}
