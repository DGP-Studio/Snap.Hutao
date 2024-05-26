// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

[SuppressMessage("", "SA1310")]
internal static class CookieExtension
{
    private const string LOGIN_TICKET = "login_ticket";
    private const string LOGIN_UID = "login_uid";
    private const string ACCOUNT_ID = "account_id";
    private const string COOKIE_TOKEN = "cookie_token";
    private const string LTOKEN = "ltoken";
    private const string LTUID = "ltuid";
    private const string MID = "mid";
    private const string STOKEN = "stoken";
    private const string STUID = "stuid";
    private const string DEVICEFP = "DEVICEFP";

    public static bool TryGetLoginTicket(this Cookie source, [NotNullWhen(true)] out Cookie? cookie)
    {
        return source.TryGetValuesToCookie([LOGIN_TICKET, LOGIN_UID], out cookie);
    }

    public static bool TryGetSToken(this Cookie source, bool isOversea, [NotNullWhen(true)] out Cookie? cookie)
    {
        return isOversea
            ? source.TryGetValuesToCookie([STOKEN, STUID], out cookie)
            : source.TryGetValuesToCookie([MID, STOKEN, STUID], out cookie);
    }

    public static bool TryGetLToken(this Cookie source, [NotNullWhen(true)] out Cookie? cookie)
    {
        return source.TryGetValuesToCookie([LTOKEN, LTUID], out cookie);
    }

    public static bool TryGetCookieToken(this Cookie source, [NotNullWhen(true)] out Cookie? cookie)
    {
        return source.TryGetValuesToCookie([ACCOUNT_ID, COOKIE_TOKEN], out cookie);
    }

    public static bool TryGetDeviceFp(this Cookie source, [NotNullWhen(true)] out string? deviceFp)
    {
        return source.TryGetValue(DEVICEFP, out deviceFp);
    }

    private static bool TryGetValuesToCookie(this Cookie source, in ReadOnlySpan<string> keys, [NotNullWhen(true)] out Cookie? cookie)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(keys.Length);
        SortedDictionary<string, string> cookieMap = [];

        foreach (ref readonly string key in keys)
        {
            if (source.TryGetValue(key, out string? value))
            {
                cookieMap.TryAdd(key, value);
            }
            else
            {
                cookie = default;
                return false;
            }
        }

        cookie = new(cookieMap);
        return true;
    }
}