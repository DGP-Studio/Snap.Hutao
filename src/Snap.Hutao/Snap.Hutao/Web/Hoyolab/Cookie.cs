// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Extension;
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
            // fix #88 自带页面登录米游社，提示登录失败
            string key = uidCounter.MaxBy(kvp => kvp.Value).Key;
            uid = inner[key];
            return true;
        }
        else
        {
            uid = null;
            return false;
        }
    }

    /// <summary>
    /// 异步尝试添加MultiToken
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>任务</returns>
    public async Task TryAddMultiTokenAsync(string uid)
    {
        if (TryGetLoginTicket(out string? loginTicket))
        {
            // get multitoken
            Dictionary<string, string> multiToken = await Ioc.Default
                .GetRequiredService<AuthClient>()
                .GetMultiTokenByLoginTicketAsync(loginTicket, uid, default)
                .ConfigureAwait(false);

            if (multiToken.Count >= 2)
            {
                inner[STUID] = uid;
                inner[STOKEN] = multiToken[STOKEN];
                inner[LTUID] = uid;
                inner[LTOKEN] = multiToken[LTOKEN];

                inner.Remove(LOGIN_TICKET);
                inner.Remove(LOGIN_UID);
            }
        }
    }

    /// <summary>
    /// 根据类型输出对应的Cookie
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>Cookie对应的字符串表示</returns>
    public string ToString(CookieType type)
    {
        IEnumerable<KeyValuePair<string, string>> results;

        results = type switch
        {
            CookieType.None => Enumerable.Empty<KeyValuePair<string, string>>(),
            CookieType.Ltoken => inner.Where(kvp => kvp.Key is E_HK4E_TOKEN or LTUID or LTOKEN or ACCOUNT_ID or COOKIE_TOKEN),
            CookieType.Stoken => inner.Where(kvp => kvp.Key is STUID or STOKEN or MID),
            CookieType.All => inner,
            _ => throw Must.NeverHappen(type.ToString()),
        };

        return string.Join(';', results.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }

    /// <summary>
    /// 转换为Cookie的字符串表示
    /// </summary>
    /// <returns>Cookie的字符串表示</returns>
    public override string ToString()
    {
        return ToString(CookieType.All);
    }
}