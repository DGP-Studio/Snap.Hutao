// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 玩家角色
/// </summary>
[HighQuality]
internal sealed class Role
{
    /// <summary>
    /// <see cref="string.Empty"/>
    /// </summary>
    [JsonPropertyName("AvatarUrl")]
    public string AvatarUrl { get; set; } = default!;

    /// <summary>
    /// 昵称
    /// </summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    /// <summary>
    /// 服务器
    /// </summary>
    [JsonPropertyName("region")]
    [JsonConverter(typeof(RegionConverter))]
    public Region Region { get; set; } = default!;

    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; } = default!;
}