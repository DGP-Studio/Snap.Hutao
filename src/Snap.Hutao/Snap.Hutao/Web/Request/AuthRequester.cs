// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json;
using Snap.Hutao.Service.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Request;

/// <summary>
/// 验证 Token 请求器
/// </summary>
public class AuthRequester : Requester
{
    /// <summary>
    /// 构造一个新的 <see cref="Requester"/> 对象
    /// </summary>
    /// <param name="httpClient">Http 客户端</param>
    /// <param name="json">Json 处理器</param>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="logger">消息器</param>
    public AuthRequester(HttpClient httpClient, Json json, IInfoBarService infoBarService, ILogger<Requester> logger)
        : base(httpClient, json, infoBarService, logger)
    {
    }

    /// <summary>
    /// 验证令牌
    /// </summary>
    public string? AuthToken { get; set; }

    /// <inheritdoc/>
    protected override void PrepareHttpClient()
    {
        base.PrepareHttpClient();
        HttpClient.DefaultRequestHeaders.Authorization = new("Bearer", AuthToken);
    }
}
