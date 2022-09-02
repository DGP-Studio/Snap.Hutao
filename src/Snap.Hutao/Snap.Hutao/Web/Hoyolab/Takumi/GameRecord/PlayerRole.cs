// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 玩家的主角信息
/// </summary>
public class PlayerRole
{
    /// <summary>
    /// 角色图标Url
    /// 总是 <see cref="string.Empty"/>
    /// </summary>
    [JsonPropertyName("AvatarUrl")]
    public string AvatarUrl { get; set; } = default!;

    /// <summary>
    /// 昵称
    /// </summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    /// <summary>
    /// 区域代码
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = default!;

    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; } = default!;
}