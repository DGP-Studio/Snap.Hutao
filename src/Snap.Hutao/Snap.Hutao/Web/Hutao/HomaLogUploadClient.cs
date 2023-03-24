// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hutao.Log;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao;

/// <summary>
/// 胡桃日志客户端
/// </summary>
[HighQuality]
[HttpClient(HttpClientConfigration.Default)]
internal sealed class HomaLogUploadClient
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// 构造一个新的胡桃日志客户端
    /// </summary>
    /// <param name="httpClient">Http客户端</param>
    public HomaLogUploadClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <summary>
    /// 上传日志
    /// </summary>
    /// <param name="exception">异常</param>
    /// <returns>任务</returns>
    public async Task<string?> UploadLogAsync(Exception exception)
    {
        HutaoLog log = BuildFromException(exception);

        Response<string>? a = await httpClient
            .TryCatchPostAsJsonAsync<HutaoLog, Response<string>>(HutaoEndpoints.HutaoLogUpload, log)
            .ConfigureAwait(false);
        return a?.Data;
    }

    private static HutaoLog BuildFromException(Exception exception)
    {
        return new()
        {
            Id = Core.CoreEnvironment.HutaoDeviceId,
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            Info = exception.ToString(),
        };
    }
}
