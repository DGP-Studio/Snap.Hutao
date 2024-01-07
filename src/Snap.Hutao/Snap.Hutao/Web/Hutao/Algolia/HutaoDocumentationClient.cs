// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Algolia;

[HttpClient(HttpClientConfiguration.Default)]
[ConstructorGenerated(ResolveHttpClient = true)]
internal sealed partial class HutaoDocumentationClient
{
    private const string AlgoliaApiKey = "72d7a9a0f9f0466218ea19988886dce8";
    private const string AlgoliaApplicationId = "28CTGDOOQD";
    private const string AlgolianetIndexesQueries = $"https://28ctgdooqd-1.algolianet.com/1/indexes/*/queries";

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HutaoDocumentationClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<AlgoliaResponse?> QueryAsync(string query, string language, CancellationToken token = default)
    {
        AlgoliaRequestsWrapper data = new()
        {
            Requests =
            [
                new AlgoliaRequest
                {
                    Query = query,
                    IndexName = "hutao",
                    Params = $"""facetFilters=["lang:{language}"]""",
                },
            ],
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(AlgolianetIndexesQueries)
            .SetHeader("x-algolia-api-key", AlgoliaApiKey)
            .SetHeader("x-algolia-application-id", AlgoliaApplicationId)
            .PostJson(data);

        return await builder.TryCatchSendAsync<AlgoliaResponse>(httpClient, logger, token).ConfigureAwait(false);
    }
}