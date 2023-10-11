// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Log;

/// <summary>
/// 胡桃日志客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HomaLogUploadClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HomaLogUploadClient> logger;
    private readonly RuntimeOptions runtimeOptions;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 上传日志
    /// </summary>
    /// <param name="exception">异常</param>
    /// <returns>任务</returns>
    public async ValueTask<string?> UploadLogAsync(Exception exception)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.HutaoLogUpload)
            .PostJson(BuildFromException(exception));

        Response<string>? resp = await builder
            .TryCatchSendAsync<Response<string>>(httpClient, logger, default)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    private HutaoLog BuildFromException(Exception exception)
    {
        return new()
        {
            Id = runtimeOptions.DeviceId,
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            Info = Core.ExceptionService.ExceptionFormat.Format(exception),
        };
    }
}
