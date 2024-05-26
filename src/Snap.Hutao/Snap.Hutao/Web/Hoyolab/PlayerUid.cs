// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

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
    public readonly Region Region;

    /// <summary>
    /// 构造一个新的玩家 Uid 结构
    /// </summary>
    /// <param name="value">uid</param>
    /// <param name="region">服务器，当提供该参数时会无条件信任</param>
    public PlayerUid(string value, in Region? region = default)
    {
        HutaoException.ThrowIfNot(HoyolabRegex.UidRegex().IsMatch(value), SH.WebHoyolabInvalidUid);
        Value = value;
        Region = region ?? Region.UnsafeFromUidString(value);
    }

    public static implicit operator PlayerUid(string source)
    {
        return FromUidString(source);
    }

    public static PlayerUid FromUidString(string uid)
    {
        return new(uid);
    }

    public static bool IsOversea(string uid)
    {
        HutaoException.ThrowIfNot(HoyolabRegex.UidRegex().IsMatch(uid), SH.WebHoyolabInvalidUid);

        return uid.AsSpan()[^9] switch
        {
            >= '1' and <= '5' => false,
            _ => true,
        };
    }

    public static TimeSpan GetRegionTimeZoneUtcOffsetForUid(string uid)
    {
        HutaoException.ThrowIfNot(HoyolabRegex.UidRegex().IsMatch(uid), SH.WebHoyolabInvalidUid);

        // 美服 UTC-05
        // 欧服 UTC+01
        // 其他 UTC+08
        return uid.AsSpan()[^9] switch
        {
            '6' => ServerRegionTimeZone.AmericaServerOffset,
            '7' => ServerRegionTimeZone.EuropeServerOffset,
            _ => ServerRegionTimeZone.CommonOffset,
        };
    }

    public static TimeSpan GetRegionTimeZoneUtcOffsetForRegion(in Region region)
    {
        // 美服 UTC-05
        // 欧服 UTC+01
        // 其他 UTC+08
        return region.Value switch
        {
            "os_usa" => ServerRegionTimeZone.AmericaServerOffset,
            "os_euro" => ServerRegionTimeZone.EuropeServerOffset,
            _ => ServerRegionTimeZone.CommonOffset,
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Value;
    }
}