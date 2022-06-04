// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 用户游戏角色
/// </summary>
public record UserGameRole
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
    public string IsOfficial { get; set; } = default!;

    /// <summary>
    /// 转化为 <see cref="PlayerUid"/>
    /// </summary>
    /// <returns>一个等价的 <see cref="PlayerUid"/> 实例</returns>
    public PlayerUid AsPlayerUid()
    {
        return new PlayerUid(GameUid, Region);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Nickname} | {RegionName} | Lv.{Level}";
    }
}