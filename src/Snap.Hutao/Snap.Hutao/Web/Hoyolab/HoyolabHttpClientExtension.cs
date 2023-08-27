// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// Hoyolab HttpClient 拓展
/// </summary>
[HighQuality]
internal static class HoyolabHttpClientExtension
{
    /// <summary>
    /// 设置用户的 Cookie
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="user">实体用户</param>
    /// <param name="cookie">Cookie类型</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetUser(this HttpClient httpClient, Model.Entity.User user, CookieType cookie)
    {
        httpClient.DefaultRequestHeaders.Remove("Cookie");
        StringBuilder stringBuilder = new();

        if (cookie.HasFlag(CookieType.CookieToken))
        {
            stringBuilder.Append(user.CookieToken).AppendIf(user.CookieToken is not null, ';');
        }

        if (cookie.HasFlag(CookieType.LToken))
        {
            stringBuilder.Append(user.LToken).AppendIf(user.LToken is not null, ';');
        }

        if (cookie.HasFlag(CookieType.SToken))
        {
            stringBuilder.Append(user.SToken).AppendIf(user.SToken is not null, ';');
        }

        string result = stringBuilder.ToString();
        if (!string.IsNullOrWhiteSpace(result))
        {
            httpClient.DefaultRequestHeaders.Set("Cookie", result);
        }

        return httpClient;
    }

    /// <summary>
    /// 设置Referer
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="referer">用户</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetReferer(this HttpClient httpClient, string referer)
    {
        httpClient.DefaultRequestHeaders.Set("Referer", referer);
        return httpClient;
    }

    /// <summary>
    /// 设置验证流水号
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="challenge">验证流水号</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetXrpcChallenge(this HttpClient httpClient, string challenge)
    {
        httpClient.DefaultRequestHeaders.Set("x-rpc-challenge", challenge);
        return httpClient;
    }

    internal static HttpClient SetXrpcChallenge(this HttpClient httpClient, string challenge, string validate)
    {
        httpClient.DefaultRequestHeaders.Set("x-rpc-challenge", challenge);
        httpClient.DefaultRequestHeaders.Set("x-rpc-validate", validate);
        httpClient.DefaultRequestHeaders.Set("x-rpc-seccode", $"{validate}|jordan");
        return httpClient;
    }

    /// <summary>
    /// 设置头
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetHeader(this HttpClient httpClient, string key, string value)
    {
        httpClient.DefaultRequestHeaders.Set(key, value);
        return httpClient;
    }
}