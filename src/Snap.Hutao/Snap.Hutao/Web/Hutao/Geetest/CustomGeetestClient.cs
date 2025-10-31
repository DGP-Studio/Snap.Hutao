// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Collections.Frozen;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Hutao.Geetest;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class CustomGeetestClient
{
    private static readonly FrozenSet<string> ImpossibleHosts =
    [
        "webstatic.mihoyo.com",
        "www.miyoushe.com",
    ];

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial CustomGeetestClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<GeetestResponse> VerifyAsync(string gt, string challenge, CancellationToken token)
    {
        string template = appOptions.GeetestCustomCompositeUrl.Value;

        try
        {
            UriBuilder uriBuilder = new(template);
            if (ImpossibleHosts.Contains(uriBuilder.Host))
            {
                await taskContext.SwitchToMainThreadAsync();
                appOptions.GeetestCustomCompositeUrl.Value = string.Empty;
                return GeetestResponse.InternalFailure;
            }
        }
        catch
        {
            return GeetestResponse.InternalFailure;
        }

        string url;
        try
        {
            CompositeFormat format = CompositeFormat.Parse(template);
            if (format.MinimumArgumentCount < 2)
            {
                // If there are less than 2 arguments, we cannot format the string correctly.
                return GeetestResponse.InternalFailure;
            }

            url = string.Format(CultureInfo.CurrentCulture, format.Format, gt, challenge);
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
            .SendAsync<GeetestResponse>(httpClient, false, token)
            .ConfigureAwait(false);

        return resp ?? GeetestResponse.InternalFailure;
    }
}