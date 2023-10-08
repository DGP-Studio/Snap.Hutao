// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Web.Hoyolab.Passport;

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 封装了米哈游的Cookie
/// </summary>
[HighQuality]
internal sealed partial class Cookie
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

    public string this[string key]
    {
        get => inner[key];
        set => inner[key] = value;
    }

    /// <summary>
    /// 解析Cookie字符串
    /// </summary>
    /// <param name="cookieString">cookie字符串</param>
    /// <returns>新的Cookie对象</returns>
    public static Cookie Parse(string cookieString)
    {
        SortedDictionary<string, string> cookieMap = new();
        cookieString = cookieString.Replace(" ", string.Empty, StringComparison.Ordinal);
        string[] values = cookieString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (string[] parts in values.Select(c => c.Split('=', 2)))
        {
            string name = parts[0].Trim();
            string value = parts.Length == 1 ? string.Empty : parts[1].Trim();
            cookieMap.TryAdd(name, value);
        }

        return new(cookieMap);
    }

    public static Cookie FromCoreWebView2Cookies(IReadOnlyList<CoreWebView2Cookie> webView2Cookies)
    {
        SortedDictionary<string, string> cookieMap = new();

        foreach (CoreWebView2Cookie cookie in webView2Cookies)
        {
            cookieMap.TryAdd(cookie.Name, cookie.Value);
        }

        return new(cookieMap);
    }

    /// <summary>
    /// 从登录结果创建 Cookie
    /// </summary>
    /// <param name="loginResult">登录结果</param>
    /// <returns>Cookie</returns>
    public static Cookie FromLoginResult(LoginResult? loginResult)
    {
        if (loginResult is null)
        {
            return new();
        }

        SortedDictionary<string, string> cookieMap = new()
        {
            [STUID] = loginResult.UserInfo.Aid,
            [STOKEN] = loginResult.Token.Token,
            [MID] = loginResult.UserInfo.Mid,
        };

        return new(cookieMap);
    }

    /// <summary>
    /// 从 SToken 创建 Cookie
    /// </summary>
    /// <param name="stuid">stuid</param>
    /// <param name="stoken">stoken</param>
    /// <returns>Cookie</returns>
    public static Cookie FromSToken(string stuid, string stoken)
    {
        SortedDictionary<string, string> cookieMap = new()
        {
            [STUID] = stuid,
            [STOKEN] = stoken,
        };

        return new(cookieMap);
    }

    public bool IsEmpty()
    {
        return inner.Count <= 0;
    }

    public bool TryGetLoginTicket([NotNullWhen(true)] out Cookie? cookie)
    {
        if (TryGetValue(LOGIN_TICKET, out string? loginTicket) && TryGetValue(LOGIN_UID, out string? loginUid))
        {
            cookie = new Cookie(new()
            {
                [LOGIN_TICKET] = loginTicket,
                [LOGIN_UID] = loginUid,
            });

            return true;
        }

        cookie = null;
        return false;
    }

    public bool TryGetSToken(bool isOversea, [NotNullWhen(true)] out Cookie? cookie)
    {
        return isOversea ? TryGetLegacySToken(out cookie) : TryGetSToken(out cookie);
    }

    public bool TryGetLToken([NotNullWhen(true)] out Cookie? cookie)
    {
        if (TryGetValue(LTOKEN, out string? ltoken) && TryGetValue(LTUID, out string? ltuid))
        {
            cookie = new Cookie(new()
            {
                [LTOKEN] = ltoken,
                [LTUID] = ltuid,
            });

            return true;
        }

        cookie = null;
        return false;
    }

    public bool TryGetCookieToken([NotNullWhen(true)] out Cookie? cookie)
    {
        if (TryGetValue(ACCOUNT_ID, out string? accountId) && TryGetValue(COOKIE_TOKEN, out string? cookieToken))
        {
            cookie = new Cookie(new()
            {
                [ACCOUNT_ID] = accountId,
                [COOKIE_TOKEN] = cookieToken,
            });

            return true;
        }

        cookie = null;
        return false;
    }

    /// <inheritdoc cref="Dictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        return inner.TryGetValue(key, out value);
    }

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>值或默认值</returns>
    public string? GetValueOrDefault(string key)
    {
        return inner.GetValueOrDefault(key);
    }

    /// <summary>
    /// 转换为Cookie的字符串表示
    /// </summary>
    /// <returns>Cookie的字符串表示</returns>
    public override string ToString()
    {
        return string.Join(';', inner.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }

    private bool TryGetSToken([NotNullWhen(true)] out Cookie? cookie)
    {
        if (TryGetValue(MID, out string? mid) && TryGetValue(STOKEN, out string? stoken) && TryGetValue(STUID, out string? stuid))
        {
            cookie = new Cookie(new()
            {
                [MID] = mid,
                [STOKEN] = stoken,
                [STUID] = stuid,
            });

            return true;
        }

        cookie = null;
        return false;
    }

    private bool TryGetLegacySToken([NotNullWhen(true)] out Cookie? cookie)
    {
        if (TryGetValue(STOKEN, out string? stoken) && TryGetValue(STUID, out string? stuid))
        {
            cookie = new Cookie(new()
            {
                [STOKEN] = stoken,
                [STUID] = stuid,
            });

            return true;
        }

        cookie = null;
        return false;
    }
}