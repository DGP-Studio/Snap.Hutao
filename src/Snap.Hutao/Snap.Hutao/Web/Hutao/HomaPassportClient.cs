// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hutao.Log;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao;

/// <summary>
/// 胡桃通行证客户端
/// </summary>
[HighQuality]
[HttpClient(HttpClientConfigration.Default)]
internal sealed class HomaPassportClient
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// 构造一个新的胡桃通行证客户端
    /// </summary>
    /// <param name="httpClient">Http客户端</param>
    public HomaPassportClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <summary>
    /// 异步获取验证码
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>响应</returns>
    public async Task<Response.Response> VerifyAsync(string email)
    {

    }
}