// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 动态密钥扩展
/// </summary>
[HighQuality]
internal static class HttpClientDynamicSecretExtensions
{
    /// <summary>
    /// 使用动态密钥
    /// </summary>
    /// <param name="client">客户端</param>
    /// <param name="version">版本</param>
    /// <param name="salt">盐</param>
    /// <param name="includeChars">包括大小写英文字母</param>
    /// <returns>同一个客户端</returns>
    public static HttpClient UseDynamicSecret(this HttpClient client, DynamicSecretVersion version, SaltType salt, bool includeChars)
    {
        client.DefaultRequestHeaders.Set(DynamicSecretHandler.OptionKeyName, $"{version}|{salt}|{includeChars}");
        return client;
    }
}