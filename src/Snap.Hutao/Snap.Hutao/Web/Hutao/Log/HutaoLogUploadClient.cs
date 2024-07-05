// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Log;

/// <summary>
/// 胡桃日志客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoLogUploadClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HutaoLogUploadClient> logger;
    private readonly RuntimeOptions runtimeOptions;
    private readonly HttpClient httpClient;

    public void UploadLog(Exception exception)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.HutaoLogUpload)
            .PostJson(BuildFromException(exception));

        builder.Send(httpClient, logger);
    }

    private HutaoLog BuildFromException(Exception exception)
    {
        return new()
        {
            Id = runtimeOptions.DeviceId,
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Info = Core.ExceptionService.ExceptionFormat.Format(exception),
        };
    }
}
