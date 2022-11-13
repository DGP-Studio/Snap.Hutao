// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 通行证客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class PassportClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<PassportClient> logger;

    /// <summary>
    /// 构造一个新的通行证客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    public PassportClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<PassportClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步验证Ltoken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>验证信息</returns>
    [ApiInformation(Cookie = CookieType.All)]
    public async Task<VerifyInformation?> VerifyLtokenAsync(User user, CancellationToken token)
    {
        Response<VerifyInformation>? response = await httpClient
            .SetUser(user, CookieType.All)
            .TryCatchPostAsJsonAsync<Timestamp, Response<VerifyInformation>>(ApiEndpoints.AccountVerifyLtoken, new(), options, logger, token)
            .ConfigureAwait(false);

        return response?.Data;
    }

    private class Timestamp
    {
        public Timestamp()
        {
            T = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        [JsonPropertyName("t")]
        public long T { get; set; }
    }
}