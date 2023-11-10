// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 玩家 Uid
/// </summary>
[HighQuality]
internal readonly partial struct PlayerUid
{
    /// <summary>
    /// UID 的实际值
    /// </summary>
    public readonly string Value;

    /// <summary>
    /// 地区代码
    /// </summary>
    public readonly string Region;

    /// <summary>
    /// 构造一个新的玩家 Uid 结构
    /// </summary>
    /// <param name="value">uid</param>
    /// <param name="region">服务器，当提供该参数时会无条件信任</param>
    public PlayerUid(string value, string? region = default)
    {
        Must.Argument(UidRegex().IsMatch(value), SH.WebHoyolabInvalidUid);
        Value = value;
        Region = region ?? EvaluateRegion(value.AsSpan()[0]);
    }

    public static implicit operator PlayerUid(string source)
    {
        return FromUidString(source);
    }

    public static PlayerUid FromUidString(string uid)
    {
        return new(uid);
    }

    /// <summary>
    /// 判断是否为国际服
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>是否为国际服</returns>
    public static bool IsOversea(string uid)
    {
        // We make this a static method rather than property,
        // to avoid unnecessary memory allocation (Region field).
        Must.Argument(UidRegex().IsMatch(uid), SH.WebHoyolabInvalidUid);

        return uid.AsSpan()[0] switch
        {
            >= '1' and <= '5' => false,
            _ => true,
        };
    }

    public static TimeSpan GetRegionTimeZoneUtcOffset(string uid)
    {
        // We make this a static method rather than property,
        // to avoid unnecessary memory allocation (Region field).
        Must.Argument(UidRegex().IsMatch(uid), SH.WebHoyolabInvalidUid);

        // 美服 UTC-05
        // 欧服 UTC+01
        // 其他 UTC+08
        return uid.AsSpan()[0] switch
        {
            '6' => ServerRegionTimeZone.AmericaServerOffset,
            '7' => ServerRegionTimeZone.EuropeServerOffset,
            _ => ServerRegionTimeZone.CommonOffset,
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Value;
    }

    private static string EvaluateRegion(in char first)
    {
        return first switch
        {
            // CN
            >= '1' and <= '4' => "cn_gf01", // 国服
            '5' => "cn_qd01",               // 渠道

            // OS
            '6' => "os_usa",                // 美服
            '7' => "os_euro",               // 欧服
            '8' => "os_asia",               // 亚服
            '9' => "os_cht",                // 台服
            _ => throw Must.NeverHappen(),
        };
    }

    [GeneratedRegex("^[1-9][0-9]{8}$")]
    private static partial Regex UidRegex();
}