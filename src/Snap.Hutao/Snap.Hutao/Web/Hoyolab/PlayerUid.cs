// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.QueryString;

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 玩家 Uid
/// </summary>
[HighQuality]
internal readonly struct PlayerUid
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
        Must.Argument(value.Length == 9, "uid 应为9位数字");
        Value = value;
        Region = region ?? EvaluateRegion(value.AsSpan()[0]);
    }

    public static implicit operator PlayerUid(string source)
    {
        return new(source);
    }

    /// <summary>
    /// 判断是否为国际服
    /// We make this a static method rather than property,
    /// to avoid unnecessary memory allocation.
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>是否为国际服</returns>
    public static bool IsOversea(string uid)
    {
        return uid[0] switch
        {
            >= '1' and <= '5' => false,
            _ => true,
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// 转换到查询字符串
    /// </summary>
    /// <returns>查询字符串</returns>
    public QueryString ToQueryString()
    {
        QueryString queryString = new();
        queryString.Set("role_id", Value);
        queryString.Set("server", Region);

        return queryString;
    }

    private static string EvaluateRegion(char first)
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
}