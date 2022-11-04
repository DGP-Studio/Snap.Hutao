// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hutao.Log;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao;

/// <summary>
/// 胡桃日志客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class HomaClient2
{
    private const string HutaoAPI = "https://homa.snapgenshin.com";
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;

    /// <summary>
    /// 构造一个新的胡桃日志客户端
    /// </summary>
    /// <param name="httpClient">Http客户端</param>
    /// <param name="options">Json序列化选项</param>
    public HomaClient2(HttpClient httpClient, JsonSerializerOptions options)
    {
        this.httpClient = httpClient;
        this.options = options;
    }

    /// <summary>
    /// 上传日志
    /// </summary>
    /// <param name="exception">异常</param>
    /// <returns>任务</returns>
    public async Task<string?> UploadLogAsync(Exception exception)
    {
        HutaoLog log = new()
        {
            Id = Core.CoreEnvironment.HutaoDeviceId,
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            Info = exception.ToString(),
        };

        Response<string>? a = await httpClient
            .TryCatchPostAsJsonAsync<HutaoLog, Response<string>>($"{HutaoAPI}/HutaoLog/Upload", log, options)
            .ConfigureAwait(false);
        return a?.Data;
    }
}