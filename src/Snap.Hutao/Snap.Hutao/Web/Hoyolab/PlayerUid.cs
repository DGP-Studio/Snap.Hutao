// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 玩家 Uid
/// </summary>
public struct PlayerUid
{
    private string? region = null;

    /// <summary>
    /// 构造一个新的玩家 Uid 结构
    /// </summary>
    /// <param name="value">uid</param>
    /// <param name="region">服务器，当提供该参数时会无条件信任</param>
    public PlayerUid(string value, string? region = default)
    {
        Requires.Argument(value.Length == 9, nameof(value), "uid应为9位数字");
        Value = value;

        if (region != null)
        {
            this.region = region;
        }
    }

    /// <summary>
    /// UID 的实际值
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// 地区代码
    /// </summary>
    public string Region
    {
        get
        {
            region ??= EvaluateRegion(Value[0]);
            return region;
        }
    }

    private static string EvaluateRegion(char first)
    {
        return first switch
        {
            >= '1' and <= '4' => "cn_gf01", // 国服
            '5' => "cn_qd01",               // 渠道
            '6' => "os_usa",                // 美服
            '7' => "os_euro",               // 欧服
            '8' => "os_asia",               // 亚服
            '9' => "os_cht",                // 台服
            _ => Must.NeverHappen<string>(),
        };
    }
}