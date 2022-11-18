// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 封装了米哈游的Cookie
/// </summary>
public partial class Cookie
{
    private readonly SortedDictionary<string, string> inner;

    /// <summary>
    /// 构造一个空白的Cookie
    /// </summary>
    public Cookie()
        : this(new())
    {
    }

    /// <summary>
    /// 构造一个新的Cookie
    /// </summary>
    /// <param name="dict">源</param>
    private Cookie(SortedDictionary<string, string> dict)
    {
        inner = dict;
    }

    /// <summary>
    /// 解析Cookie字符串
    /// </summary>
    /// <param name="cookieString">cookie字符串</param>
    /// <returns>新的Cookie对象</returns>
    public static Cookie Parse(string cookieString)
    {
        SortedDictionary<string, string> cookieMap = new();
        cookieString = cookieString.Replace(" ", string.Empty);
        string[] values = cookieString.TrimEnd(';').Split(';');
        foreach (string[] parts in values.Select(c => c.Split('=', 2)))
        {
            string name = parts[0].Trim();
            string value = parts.Length == 1 ? string.Empty : parts[1].Trim();

            cookieMap.Add(name, value);
        }

        return new(cookieMap);
    }

    public static Cookie FromCoreWebView2Cookies(IReadOnlyList<CoreWebView2Cookie> webView2Cookies)
    {
        SortedDictionary<string, string> cookieMap = new();

        foreach (CoreWebView2Cookie cookie in webView2Cookies)
        {
            cookieMap.Add(cookie.Name, cookie.Value);
        }

        return new(cookieMap);
    }

    /// <summary>
    /// 此 Cookie 是 SToken
    /// </summary>
    /// <returns>是否存在</returns>
    public bool IsStoken()
    {
        int stokenFlag = 0;

        foreach (string key in inner.Keys)
        {
            if (key is MID or STOKEN or STUID)
            {
                stokenFlag++;
            }
        }

        return stokenFlag == 3;
    }

    /// <summary>
    /// 异步获取 Mid
    /// </summary>
    /// <returns>mid</returns>
    public async Task<string?> GetMidAsync()
    {
        string? mid;
        if (IsStoken())
        {
            mid = inner[MID];
        }
        else
        {
            UserInformation? userInfo = await Ioc.Default
                .GetRequiredService<PassportClient>()
                .VerifyLtokenAsync(this, CancellationToken.None)
                .ConfigureAwait(false);

            mid = userInfo?.Mid;
        }

        return mid;
    }

    /// <inheritdoc cref="Dictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        return inner.TryGetValue(key, out value);
    }

    /// <summary>
    /// 转换为Cookie的字符串表示
    /// </summary>
    /// <returns>Cookie的字符串表示</returns>
    public override string ToString()
    {
        return string.Join(';', inner.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
}