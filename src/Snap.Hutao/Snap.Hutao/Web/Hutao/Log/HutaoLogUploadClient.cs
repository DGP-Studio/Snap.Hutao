// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Log;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoLogUploadClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly ILogger<HutaoLogUploadClient> logger;
    private readonly HttpClient httpClient;

    public void UploadLog(Exception exception)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().HutaoLogUpload())
            .PostJson(BuildFromException(exception));

        builder.Send(httpClient, logger);
    }

    private static HutaoLog BuildFromException(Exception exception)
    {
        return new()
        {
            Id = HutaoRuntime.DeviceId,
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Info = Core.ExceptionService.ExceptionFormat.Format(exception),
        };
    }
}
