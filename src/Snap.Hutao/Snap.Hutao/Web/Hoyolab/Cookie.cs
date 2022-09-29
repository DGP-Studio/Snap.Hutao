// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Extension;

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
    /// 存在 LoginTicket
    /// </summary>
    /// <returns>是否存在</returns>
    public bool ContainsLoginTicket()
    {
        return inner.ContainsKey(LOGIN_TICKET);
    }

    /// <summary>
    /// 存在 LToken 与 CookieToken
    /// </summary>
    /// <returns>是否存在</returns>
    public bool ContainsLTokenAndCookieToken()
    {
        return inner.ContainsKey(LTOKEN) && inner.ContainsKey(COOKIE_TOKEN);
    }

    /// <summary>
    /// 存在 SToken
    /// </summary>
    /// <returns>是否存在</returns>
    public bool ContainsSToken()
    {
        return inner.ContainsKey(STOKEN);
    }

    /// <summary>
    /// 插入Stoken
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="multiToken">tokens</param>
    public void InsertMultiToken(string uid, Dictionary<string, string> multiToken)
    {
        inner[STUID] = uid;
        inner[STOKEN] = multiToken[STOKEN];

        inner[LTUID] = uid;
        inner[LTOKEN] = multiToken[LTOKEN];
    }

    /// <summary>
    /// 插入 Stoken
    /// </summary>
    /// <param name="stuid">stuid</param>
    /// <param name="cookie">cookie</param>
    public void InsertSToken(string stuid, Cookie cookie)
    {
        inner[STUID] = stuid;
        inner[STOKEN] = cookie.inner[STOKEN];
    }

    /// <summary>
    /// 移除 LoginTicket
    /// </summary>
    public void RemoveLoginTicket()
    {
        inner.Remove(LOGIN_TICKET);
        inner.Remove(LOGIN_UID);
    }

    /// <summary>
    /// 移除无效的键
    /// </summary>
    public void Trim()
    {
        foreach (string key in inner.Keys.ToList())
        {
            if (key == ACCOUNT_ID
                || key == COOKIE_TOKEN
                || key == LOGIN_UID
                || key == LOGIN_TICKET
                || key == LTUID
                || key == LTOKEN
                || key == STUID
                || key == STOKEN)
            {
                continue;
            }
            else
            {
                inner.Remove(key);
            }
        }
    }

    /// <inheritdoc cref="Dictionary2{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        return inner.TryGetValue(key, out value);
    }

    public bool TryGetLoginTicket([NotNullWhen(true)] out string? loginTicket)
    {
        return inner.TryGetValue(LOGIN_TICKET, out loginTicket);
    }

    public bool TryGetUid([NotNullWhen(true)] out string? uid)
    {
        Dictionary<string, int> uidCounter = new();

        foreach ((string key, string value) in inner)
        {
            if (key is ACCOUNT_ID or LOGIN_UID or LTUID or STUID)
            {
                uidCounter.Increase(key);
            }
        }

        if (uidCounter.Count > 0)
        {
            uid = uidCounter.MaxBy(kvp => kvp.Value).Key;
            return true;
        }
        else
        {
            uid = null;
            return false;
        }
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

/// <summary>
/// 键部分
/// </summary>
[SuppressMessage("", "SA1310")]
[SuppressMessage("", "SA1600")]
public partial class Cookie
{
    public const string COOKIE_TOKEN = "cookie_token";
    public const string ACCOUNT_ID = "account_id";

    public const string LOGIN_TICKET = "login_ticket";
    public const string LOGIN_UID = "login_uid";

    public const string LTOKEN = "ltoken";
    public const string LTUID = "ltuid";

    public const string STOKEN = "stoken";
    public const string STUID = "stuid";
}