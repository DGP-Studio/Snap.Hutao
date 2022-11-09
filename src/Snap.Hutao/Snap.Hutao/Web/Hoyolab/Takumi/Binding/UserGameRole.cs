// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 用户游戏角色
/// </summary>
public class UserGameRole
{
    /// <summary>
    /// hk4e_cn for Genshin Impact
    /// </summary>
    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; } = default!;

    /// <summary>
    /// 服务器
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = default!;

    /// <summary>
    /// 游戏Uid
    /// </summary>
    [JsonPropertyName("game_uid")]
    public string GameUid { get; set; } = default!;

    /// <summary>
    /// 昵称
    /// </summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 是否选中
    /// </summary>
    [JsonPropertyName("is_chosen")]
    public bool IsChosen { get; set; }

    /// <summary>
    /// 地区名称
    /// </summary>
    [JsonPropertyName("region_name")]
    public string RegionName { get; set; } = default!;

    /// <summary>
    /// 是否为官服
    /// </summary>
    [JsonPropertyName("is_official")]
    public bool IsOfficial { get; set; } = default!;

    /// <summary>
    /// 玩家服务器与等级简述
    /// </summary>
    public string Description
    {
        get => $"{RegionName} | Lv.{Level}";
    }

    public static implicit operator PlayerUid(UserGameRole userGameRole)
    {
        return new PlayerUid(userGameRole.GameUid, userGameRole.Region);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Nickname} | {RegionName} | Lv.{Level}";
    }
}