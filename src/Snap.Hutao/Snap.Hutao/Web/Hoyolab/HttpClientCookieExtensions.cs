// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// <see cref="HttpClient"/> 扩展
/// </summary>
internal static class HttpClientCookieExtensions
{
    /// <summary>
    /// 设置用户的Cookie
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="user">用户</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetUser(this HttpClient httpClient, User user)
    {
        if (!User.IsNone(user))
        {
            httpClient.DefaultRequestHeaders.Set("Cookie", user.Cookie);
        }

        return httpClient;
    }
}
